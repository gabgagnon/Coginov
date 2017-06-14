
var langFrench = {
    "sProcessing": "Traitement en cours...",
    "sSearch": "Rechercher&nbsp;:",
    "sLengthMenu": "Afficher _MENU_ &eacute;l&eacute;ments",
    "sInfo": "Affichage de l'&eacute;l&eacute;ment _START_ &agrave; _END_ sur _TOTAL_ &eacute;l&eacute;ments",
    "sInfoEmpty": "Affichage de l'&eacute;l&eacute;ment 0 &agrave; 0 sur 0 &eacute;l&eacute;ment",
    "sInfoFiltered": "(filtr&eacute; de _MAX_ &eacute;l&eacute;ments au total)",
    "sInfoPostFix": "",
    "sLoadingRecords": "Chargement en cours...",
    "sZeroRecords": "Aucun &eacute;l&eacute;ment &agrave; afficher",
    "sEmptyTable": "Aucune donn&eacute;e disponible dans le tableau",
    "oPaginate": {
        "sFirst": "Premier",
        "sPrevious": "Pr&eacute;c&eacute;dent",
        "sNext": "Suivant",
        "sLast": "Dernier"
    },
    "oAria": {
        "sSortAscending": ": activer pour trier la colonne par ordre croissant",
        "sSortDescending": ": activer pour trier la colonne par ordre d&eacute;croissant"
    }

}

$('#fileCount .body').text(auditData.AuditedFiles.length);
$('#fileSize .body').text(filesize(_(auditData.AuditedFiles).map('Size').sum()));
$('#sites .body').text(_(auditData.AuditedFiles).map('WebUrl').uniq().value().length);

/// Graph count par extensions

var sortedByExt = _(auditData.AuditedFiles)
    .groupBy(function (i) { return i.Path.substring(i.Path.lastIndexOf('.') + 1); })
    .entries()
    .orderBy(function (k) { return k[1].length; }, 'desc');

$(function () {
    var filesByExtCntnr = $('#filesByExt > div.graph');
    filesByExtCntnr.highcharts({
        chart: { type: 'bar' },
        title: { text: filesByExtCntnr.siblings('.title').hide().text() },
        xAxis: {
            categories: sortedByExt.map(0).value(),
            title: { text: null }
        },
        yAxis: {
            min: 0,
            title: { text: null },
            labels: { overflow: 'justify' }
        },
        legend: { enabled: false },
        plotOptions: {
            bar: { dataLabels: { enabled: true } }
        },
        credits: { enabled: false },
        series: [{
            name: 'File extensions',
            data: sortedByExt.map(function (i) { return i[1].length }).value()
        }]
    });
});

/// Graph size v age v count

var fileAge = _(auditData.AuditedFiles)
    .groupBy(function (i) { return i.LastModified.substring(0, 4); })
    .entries()
    .map(function (k) { return [k[0], k[1].length, _.sumBy(k[1], 'Size')]; });

$(function () {
    var filesByExtCntnr = $('#fileAgeing > div.graph');
    filesByExtCntnr.highcharts({
        chart: {
            type: 'column'
        },
        title: {
            text: filesByExtCntnr.siblings('.title').hide().text()
        },
        xAxis: {
            categories: fileAge.map(0).value(),
            title: {
                text: 'Année'
            }
        },
        yAxis: [
            { title: { text: 'Nombre' } },
            { title: { text: 'Taille' }, labels: { format: '{value} bytes' }, opposite: true }],
        legend: {
            enabled: true
        },
        tooltip: {
            enabled: true
        },
        plotOptions: {
            column: {
                grouping: false,
                shadow: false,
                borderWidth: 0
            }
        },
        credits: { enabled: false },
        series: [{
            name: 'Nombre',
            color: 'rgba(165,170,217,1)',
            data: fileAge.map(1).value(),
            pointPadding: 0.3
        }, {
            name: 'Taille',
            yAxis: 1,
            color: 'rgba(126,86,134,.9)',
            data: fileAge.map(2).value(),
            pointPadding: 0.4
        }]
    });
});

/// Graph duplicate v First file name v nb

var filesDuplicate = _(auditData.Duplicates)
    .groupBy(function (i) { return i.Size; })
    .toPairs()
    .map(function (fileDuplicates) {
        return [fileDuplicates[1][0].Path, fileDuplicates[1].length, fileDuplicates[1]];
    }).value();

var filesDuplicateData = filesDuplicate.map(function (fileDuplicates) {
    var item = { name: fileDuplicates[0].split('/').pop() , value: fileDuplicates[1], path: fileDuplicates[0] }
    return item;
});
var tableDuplicate;
var cRect = { firstPath: "" };
$(function () {
    var filesByExtCntnr = $('#duplicates > div.graph');
    filesByExtCntnr.highcharts({
        series: [{
            type: "treemap",
            layoutAlgorithm: 'squarified',
            data: filesDuplicateData,
            events: {
                click: function(event) {
                    updateTable();
                }
            }
        }],
        credits: { enabled: false },
        title: {
            text: filesByExtCntnr.siblings('.title').hide().text(),
        },
        tooltip: {
            formatter: function () {
                cRect.firstPath = this.point.path;
                return this.point.value + ' fichiers doublons de <b>' + this.point.name;
            }
        }
    });
    tableDuplicate = $('#tableDuplicate').DataTable({
        "bPaginate": true,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bAutoWidth": false,
        "language": langFrench
    });
});

function updateTable() {
    var selectedId = _.findKey(filesDuplicate, function (o) { return o[0] == cRect.firstPath; });
    var selectedFileDuplicates = filesDuplicate[selectedId];
    tableDuplicate.clear();
    selectedFileDuplicates[2].forEach(function (file) {
        var fileName = file.Path.split('/').pop();
        tableDuplicate.row.add([fileName, file.Path]);
    });
    tableDuplicate.draw();
}

// Nuage de termes (wordcloud) semantique

var conceptWeigths = {};
var concepts = [];

for (var i in auditData.AuditedFiles)
{
    var f = auditData.AuditedFiles[i];
    for (var j in f.Concepts)
    {
        if (!(j in conceptWeigths))
            conceptWeigths[j] = f.Concepts[j];
        else
            conceptWeigths[j] += f.Concepts[j];
    }
}

for (var i in conceptWeigths) {
    concepts.push({ 'concept': i, 'weigth': conceptWeigths[i] });
}

concepts.sort(function (a, b) { return a.weigth - b.weigth; });

var fill = d3.scaleOrdinal(d3.schemeCategory20);

var fontSize = d3.scaleLog().range([20, 60]);
fontSize.domain([+concepts[0].weigth, +concepts[concepts.length -1].weigth || 1]);

var width = 600, height = 400; 
var layout = d3.layout.cloud()
    .size([width, height])
    .words(concepts)
    .text(function (d) {
        return d.concept;
    })
    .padding(5)
    .rotate(function () { return ~~(Math.random() * 2) * 90; })
    .font("Impact")
    .fontSize(function (d) {
        return fontSize(d.weigth);
    });

var svg = d3.select("#semanticCloud .graph").append("svg")
    .attr("width", width)
    .attr("height", height);
var background = svg.append('g');
var graph = svg.append('g').attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

var draw = function (words, e) {
    graph.selectAll("text")
        .data(words)
        .enter()
        .append("text")
        .attr("text-anchor", "middle")
        .attr("transform", function (d) {
            return "translate(" + [d.x, d.y] + ")rotate(" + d.rotate + ")";
        }).style("font-size", "1px").transition().duration(1e3).style("font-size", function (t) {
            return t.size + "px"
        }).style("font-family", function (t) {
            return t.font
        }).style("fill", function (d, i) {
            return fill(i);
        })
        .text(function (c) {
            return c.concept;
        });
};

layout.on("end", draw)
layout.start();