//--------------------------------
// populate commits page
//-------------------------------
function loadCommits() {

    var uri; // = '/api/commits?exclude=' + encodeURIComponent(getUsername1()) + '&max=30';
    var keyword = $('#inputKeyWord').val();

    if (keyword) {
        uri = '/api/commits?keyword=' + keyword;
    }
    else {
        uri = '/api/commits?exclude=' + encodeURIComponent(getUsername1()) + '&max=30';
    }
    
    $.getJSON(uri)
        .done(function (data) {
            $.each(data.Commits, function (key, item) {
                createItem(item);
            });
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
function createItem(revision) {

    var block = $('<div></div>').attr('class', 'commit-item2');
    var actions = $('<div></div>').attr('class', 'commit-actions2');

    $('<div></div>')
        .text(revision.Message)
        .attr('class', 'commit-title2')
        .appendTo(block);

    $('<div></div>')
        .text("Revision : " + revision.Revision + " by " + revision.Author + " on " + revision.Timestamp)
        .attr('class', 'commit-subtitle')
        .appendTo(block);

    $('<div></div>')
        .attr('class', 'commit-annotation-summary')
        .append($('<span></span>').attr('class', 'label label-primary').text(revision.NumberReviewers)).append(' reviewers ')
        .append($('<span></span>').attr('class', 'label label-success').text(revision.NumberComments)).append(' comments ')
        .append($('<span></span>').attr('class', 'label label-warning').text(revision.NumberReplies)).append(' replies')
        .appendTo(block);

    if (revision.ApprovedBy) {
        $('<div></div>')
            .text('Cool! Approved by: ' + revision.ApprovedBy)
            .appendTo(actions);
    }
    else if (getUsername1() !== revision.Author) {
        $('<button></button>').text('Approve').on('click', { revision: revision.Revision }, approveCommit).appendTo(actions);
    }
// IGNORE $('<button></button>').text('Ignore').on('click', ignoreCommit).appendTo(actions);
    actions.appendTo(block);

    block.on('click', { revision: revision.Revision }, openForReview);

    block.appendTo(insertPoint);
}

//
// open for review action to show revision changes and allow a review
function openForReview(event) {
    var revision = event.data.revision;
    window.location = "../review.html?revision=" + revision;
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
    var approver = getUsername1();

    var uri = '/api/commits/approve/' + revision + '/' + approver;
    $.post(uri)
        .done(function () { refreshPage(event) }); // TODO
}
