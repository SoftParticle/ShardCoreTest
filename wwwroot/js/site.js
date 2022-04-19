// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function initProductsDataTable() {
    $("#products").DataTable({
        dom: 'Bfrtip',
        buttons: [
            {
                text: 'New Product',
                className: 'btn btn-primary',
                action: function (e, dt, node, config) {
                    var url = "/Home/New"
                    window.location = url;
                }
            }
        ],
        "processing": true, // to show progress bar
        "serverSide": true, // to process server side
        "filter": true, // this is to disable filter (search box)
        "orderMulti": false, // to disable multiple column at once
        "ajax": {
            "url": "/Home/LoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs":
            [{
                "targets": [0],
                "visible": true,
                "searchable": false,
                "className": "id-column"
            },
            {
                "targets": [4, 5],
                "className": "button-column"
            }],
        "columns": [
            {
                "data": "id", "name": "Id", "autoWidth": true, "searchable": true
            },
            {
                "data": "model", "name": "Model", "autoWidth": true, "searchable": true
            },
            {
                "data": "description", "name": "Description", "autoWidth": true, "searchable": true
            },
            {
                "data": "price", "name": "Price", "autoWidth": true, "searchable": false,
                render: function (data, type, full, meta) { return data.toLocaleString('pt-PT', { style: 'currency', currency: 'EUR' }); }
            },
            {
                "render": function (data, type, full, meta) {
                    return '<a class="btn btn-info" href="/Home/Edit/' + full.id + '">Edit</a>';
                },
                "orderable": false
            },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='#' class='btn btn-danger' onclick=DeleteProductData('" + row.id + "'); >Delete</a>";
                },
                "orderable": false
            }
        ]
    });
}

function DeleteProductData(id) {
    if (confirm("Are you sure you want to delete ...?")) {
        DeleteProduct(id);
    }
    else {
        return false;
    }
}


function DeleteProduct(id) {
    var url = "/Home/Delete";

    $.post(url, { id: id }, function (data) {
        if (data) {
            oTable = $('#products').DataTable();
            oTable.draw();
        }
        else {
            alert("Something Went Wrong!");
        }
    });
}

function initShardsChart() {
    var jqxhr = $.ajax("/Home/ShardsStats")
        .done(function (data) {
            var labels = [];
            var values = [];
            var backgroundColors = ['#4e73df', '#1cc88a', '#36b9cc'];
            var hoverBackgroundColors = ['#2e59d9', '#17a673', '#2c9faf'];
            
            for (var shard in data.shardsProductsCount) {

                labels.push(shard);
                values.push(data.shardsProductsCount[shard]);
            }

            $("#total-products-container").html(data.totalProducts);

            if (data.startSeedTime != null) {
                $("#start-seed-time-container").html(formatDate(data.startSeedTime));
                $('#seed-times-container').removeAttr('hidden');
            }

            if (data.stopSeedTime != null) {
                $("#stop-seed-time-container").html(formatDate(data.stopSeedTime));
            }

            // Set new default font family and font color to mimic Bootstrap's default styling
            Chart.defaults.font.family = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
            Chart.defaults.color = '#858796';
            
            // Shards Pie Chart
            var ctx = document.getElementById("shardsPieChart");
            var shardsPieChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: labels,
                    datasets: [{
                        data: values,
                        backgroundColor: backgroundColors,
                        hoverBackgroundColor: hoverBackgroundColors,
                        hoverBorderColor: "rgba(234, 236, 244, 1)",
                    }],
                },
                options: {
                    maintainAspectRatio: false,
                    cutout: "80%",
                    plugins: {
                            tooltips: {
                            backgroundColor: "rgb(255,255,255)",
                            bodyFontColor: "#858796",
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            xPadding: 15,
                            yPadding: 15,
                            displayColors: false,
                            caretPadding: 10,
                        },
                        legend: {
                            display: true,
                            position: 'bottom',
                            labels: {
                                boxWidth: 10,
                                pointStyle: 'circle',
                                usePointStyle: true
                            }
                        }
                    }
                }
            });

        })
        .fail(function () {
            alert("Error getting shard stats");
        })
        .always(function () {
        });
}


function initSignalRPieChart() {
    "use strict";

    var connection = new signalR.HubConnectionBuilder().withUrl("/shardsStatsHub").build();

    connection.on("UpdateShardsStats", function (shardStats) {

        var chart = Chart.getChart('shardsPieChart');
        var values = [];
        for (var shard in shardStats.shardsProductsCount) {
            values.push(shardStats.shardsProductsCount[shard]);
        }

        chart.data.datasets[0].data = values;
        chart.update();
        var totalProductsPreviousValue = $("#total-products-container").html();
        $("#total-products-container").html(shardStats.totalProducts);

        if (shardStats.startSeedTime != null) {
            $("#start-seed-time-container").html(formatDate(shardStats.startSeedTime));
            $("#seed-times-container").removeAttr('hidden');
        }

        if (shardStats.stopSeedTime != null) {
            $("#stop-seed-time-container").html(formatDate(shardStats.stopSeedTime));
        }

        if (shardStats.lastBlockDurationInSeconds != null && shardStats.lastBlockDurationInSeconds != 0) {
            $("#seed-blocks-duration-list").append('<li class="list-group-item">' + totalProductsPreviousValue + ' - ' + $("#total-products-container").html() + ' - ' + (new Date(shardStats.lastBlockDurationInSeconds * 1000).toISOString().substr(11, 8)) + '</li>');
        }
    });

    connection.start().then(function (shardStats) {
    }).catch(function (err) {
        return console.error(err.toString());
    });

    $("#seed-button").on("click", function () {
        $("#blocks-duration-container").removeAttr('hidden');
        $.ajax({
            method: "POST",
            url: "/Home/SeedProducts"
        });
    });
}

function formatDate(dateString) {
    var date = new Date(dateString);
    var day = date.getDate() < 10 ? '0' + date.getDate() : date.getDate();
    var month = date.getMonth() < 10 ? '0' + (date.getMonth() + 1) : date.getMonth() + 1;
    var hours = date.getHours() < 10 ? '0' + date.getHours() : date.getHours();
    var minutes = date.getMinutes() < 10 ? '0' + date.getMinutes() : date.getMinutes();
    var formattedDate = day + "-" + month + "-" + date.getFullYear() + " " + hours + ":" + minutes;

    return formattedDate;
}

