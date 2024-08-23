<script src="https://cdn.canvasjs.com/canvasjs.min.js"></script>
<script>
    window.onload = function () {
        var colors = ["#FF6347", "#FF4500", "#FFD700", "#ADFF2F", "#00FA9A", "#1E90FF", "#BA55D3", "#FF69B4", "#8B0000", "#808080"];
        
        var expenseDataPoints = @Html.Raw(dataPoints);
        expenseDataPoints.forEach(function (dataPoint, index) {
            dataPoint.color = colors[index % colors.length]; 
        });

        var chart = new CanvasJS.Chart("chartContainer", {
            animationEnabled: true,
            title: {
                text: "Gelir ve Gider Tablosu"
            },
            data: [{
                type: "pie",
                startAngle: 240,
                yValueFormatString: "##0.00\"TL\"",
                indexLabel: "{label} {y}",
                dataPoints: expenseDataPoints,
                explodeOnClick: true  
            }]
        });
        chart.render();
    }
</script>
