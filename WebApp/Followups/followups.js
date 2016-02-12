//--------------------------------
// populate commits page
//-------------------------------
function loadFollowUps() {

    var uri = '/api/commits?include=' + encodeURIComponent(getUsername()) + '&max=30';
    var keyword = $('#inputKeyWord').val();

    if (keyword) {
        uri += '&keyword=' + keyword;
    }
    

    $.getJSON(uri)
        .done(function (data) {
            // On success, 'data' contains a list of products.
            $.each(data.Commits, function (key, item) {
                // Add a list item for the product.
                createItem(item);
            });
        });
}

function refreshPage(event) {

    $('#insertPoint').empty();
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

    if (getUsername() !== revision.Author) {
        $('<button></button>').text('Approve').on('click', approveCommit).appendTo(actions);
    }

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
// approve button handler (not yet implemented)
function approveCommit(event) {
    event.stopPropagation();
}
