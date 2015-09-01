/*
* handle commit display
*/



function loadCommits() {

    var uri = 'api/commits/since/34698/20';

    $.getJSON(uri)
        .done(function (data) {
            // On success, 'data' contains a list of products.
            $.each(data, function (key, item) {
                // Add a list item for the product.
                createItem(item);
            });
        });
}

function createItem(item) {
    //$('<li>', { html: getItemHtml(item) }).appendTo($('#commits'));

    var listItem = $('<li></li>');

    listItem.on('click', { revision: item }, selectCommit);
    
    var message = $('<div></div>');
    message.attr('class', 'commit-message');
    message.text(item.Message);
    listItem.append(message);
    listItem.appendTo($('#commits'));

}

function getItemHtml(item) {
    return '<div class="commit" onclick="javascript:selectCommit(' + item.Revision + 
        ');"><div class="commit-message">' + item.Message + 
        '</div><div class="commit-detail">' + item.Author + ' : ' + item.DateTime + '</div></div>';
}

function selectCommit(event) {
    var revision = event.data.revision;

    loadRevision(revision);
}

function loadRevision(revision) {
    var uri = 'api/commits/revision/' + revision.Revision;

    $.getJSON(uri)
        .done(function (data) {

            var insertPoint = $('#insertPoint');
            insertPoint.empty();

            $('<div></div>').text(revision.Message).attr('class', 'commit-title').appendTo(insertPoint);
            $('<div></div>').text("Revision : " + revision.Revision + " by " +revision.Author + " on " + revision.DateTime).attr('class', 'commit-subtitle').appendTo(insertPoint);

            // On success, 'data' contains a list of products.
            $.each(data, function (key, item) {
                // Add a list item for the product.
                showRevision(item);
            });
        });
}

function showRevision(item) {

    var lineIndex;
    var table = $('<table></table>').attr('class', 'revision-file-table' );
    var row = $('<tr></tr>');
    var header = $('<td></td>').attr('colspan', '4').attr('class', 'revision-file-header').text(item.Name);
    row.append(header);
    table.append(row);

    var nbLines = item.Lines.length;
    for (lineIndex = 0; lineIndex < nbLines; lineIndex++) {
        var data = item.Lines[lineIndex];
        var line = $('<tr></tr>').attr('class', 'revision-line ' + getExtraStyleForState(data.Changed));

        table.append(line);

        $('<td></td>').attr('class', 'revision-line-number ' + getExtraStyleForState(data.Changed) ).text(lineIndex).appendTo(line);
        $('<td></td>').attr('class', 'revision-line-number ' + getExtraStyleForState(data.Changed)).text(lineIndex).appendTo(line);
        $('<td></td>').attr('class', 'revision-line-state').html( getChangedSymbol(data.Changed ) ).appendTo(line);
        $('<td></td>').attr('class', 'revision-line-text').html(formatLineOfCode(data)).appendTo(line);
    }
    $('#insertPoint').append(table);
}

function getExtraStyleForState(state) {
    if (state === 0) {
        return "state-none";
    }
    else if (state === 1) {
        return "state-added";
    }

    return "state-removed";
}

function getChangedSymbol(state) {

    if (state === 0) {
        return "&nbsp;";
    }
    else if (state === 1) {
        return "+";
    }
    return "-";
}

function formatLineOfCode(line) {
    var formattedLine = "";
    var whitespace = true;
    var index = 0;
    var length = line.Text.length;

    while (index < length && whitespace) {
        if (line.Text[index] === ' ') {
            formattedLine += "&nbsp;";
            index++;
        }
        else if (line.Text[index] === '\t' ) {
            formattedLine += "&nbsp;&nbsp;&nbsp;";
            index++;
        }
        else
        {
            whitespace = false;
        }
    }

    formattedLine += line.Text.substring(index);

    return formattedLine;
}
