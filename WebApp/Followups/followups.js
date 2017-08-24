//--------------------------------
// populate commits page
//-------------------------------
function loadFollowUps() {

    var parameters = {};
    parameters.include = getUsername();
    parameters.excludeApproved = true;
    max = 100;
    

    $.getJSON('/api/commits', parameters)
        .done(function (data) {
            if (data.Commits.length == 0) {
                $('#insertPoint').empty();
                $('#insertPoint').append($('<div></div>').text('Nothing to show - looks like your up to date!'));
            }
            else {
                $.each(data.Commits, function (key, item) {
                    createItem(item);
                });
            }
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
// TODO fusion with commits.js version
function createItem(revision) {

    var block = $('<div></div>').attr('class', 'commit-item2');
    var actions = $('<div></div>').attr('class', 'commit-actions2');

    var title = $('<div></div>')
        .text(revision.Message)
        .attr('class', 'commit-title2')
        .appendTo(block);

    $('<div></div>')
       .text("Revision : " + revision.Revision + " by " + revision.Author + " on " + revision.Timestamp)
       .attr('class', 'commit-subtitle')
       .appendTo(title);

    $('<div></div>')
        .attr('class', 'commit-annotation-summary')
        .append($('<span class="label label-primary"><i class="fa fa-eye"></i> ' + revision.NumberReviewers + ' </span>'))
        .append($('<span class="label label-danger"><i class="fa fa-comment"></i> ' + revision.NumberComments + ' </span>'))
        .append($('<span class="label label-warning"><i class="fa fa-comments"></i> ' + revision.NumberReplies + ' </span>'))
        .appendTo(block);

    if (revision.ApprovedBy) {
        $('<div></div>')
            .text('Cool! Approved by: ' + revision.ApprovedBy)
            .appendTo(actions);
    }
    else if (getUsername() !== revision.Author) {
        $('<button ></button>').attr('class', 'btn btn-success').text('Approve').on('click', approveCommit).appendTo(actions);
    }
    actions.appendTo(block);

    block.on('click', { revision: revision.Revision }, openForReview);

    $('<div></div>').attr('class', 'commit-item-footer').appendTo(block);

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
