//--------------------------------
// populate commits page
//-------------------------------
function loadCommits() {

    var uri;
    var keyword = $('#inputKeyWord').val();
    var excludeApproved = $('#inputExcludeApproved')[0].checked;
    var exludeMine = $('#inputExcludeMine')[0].checked;

    var parameters = {};
    if (keyword) {
        parameters.keyword = keyword;
    }

    if (excludeApproved) {
        parameters.excludeApproved = true;
    }

    if (exludeMine) {
        parameters.exclude = getUsername();
    }

    parameters.max = 100;
   
    $.getJSON("/api/commits", parameters)
        .done(function (data) {
            if ( data.Commits.length == 0 ) {
                $('#insertPoint').empty();
                $('#insertPoint').append($('<div></div>').text('Nothing to show'));
            }
            else{
                $.each(data.Commits, function (key, item) {
                    createItem(item);
                });
             }
        });
}

function refreshPage(event) {

    $('#insertPoint').empty()
    loadCommits();
}

//
// creates a line for a commit
// - contains, commit log, revision details, review state and action buttons
//
// TODO refactoring duplicted version in followup.js
function createItem(revision) {

    var block = $('<div></div>').attr('class', 'commit-item2');

    var title = $('<div></div>')
         .text(revision.Message)
         .attr('class', 'commit-title2')
         .appendTo(block);

    $('<div></div>')
        .text("Revision : " + revision.Revision + " by " + revision.Author + " on " + revision.Timestamp)
        .attr('class', 'commit-subtitle')
        .appendTo(title);

    var annotations = $('<div></div>')
        .attr('class', 'commit-annotation-summary');

    if (revision.ApprovedBy) {
         $("<span></span>")
             .attr('class', 'label label-success')
             .append($('<i class="fa fa-check-circle"></i> '))
             .append(' ' + revision.ApprovedBy)
             .appendTo(annotations);
    }

    annotations.append($('<span class="label label-primary"><i class="fa fa-eye"></i> ' + revision.NumberReviewers + ' </span>'))
    annotations.append($('<span class="label label-danger"><i class="fa fa-comment"></i> ' + revision.NumberComments + ' </span>'))
    annotations.append($('<span class="label label-warning"><i class="fa fa-comments"></i> ' + revision.NumberReplies + ' </span>'))

    annotations.appendTo(block);

    block.on('click', { revision: revision.Revision }, openForReview);

    $('<div></div>').attr('class', 'commit-item-footer').appendTo(block);

    block.appendTo(insertPoint);
}

//
// open for review action to show revision changes and allow a review
function openForReview(event) {
    var revision = event.data.revision;
    window.location = "/review.html?revision=" + revision;
}

//
// ignore button handler (not yet implemented)
function ignoreCommit(event) {
    event.stopPropagation();
}

//
// approve button handler (not yet implemented)
function approveCommit(event) {
    event.stopPropagation();

    // TODO refactor same code in revision detail.js
    var revision = event.data.revision;
    var approver = getUsername();

    var uri = '/api/commits/approve/' + revision + '/' + approver;
    $.post(uri)
        .done(function () { refreshPage(event) }); // TODO
}
