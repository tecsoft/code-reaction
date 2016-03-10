
//
// Load the review page for the revision specified in the parameter
//
function loadReview() {

    var revision = getParameterByName('revision');
    var uri = 'api/commits/revision/' + revision;

    $.getJSON(uri)
        .done(function (revisionDetailViewModel) {

            var insertPoint = $('#insertPoint');
            insertPoint.empty();

            var block = $('<div></div>').attr('class', 'commit-item2');
            var actions = $('<div></div>').attr('class', 'commit-actions2');

            var title = $('<div></div>')
                .text(revisionDetailViewModel.Message)
                .attr('class', 'commit-title2')
                .appendTo(block);

            $('<div></div>')
               .text("Revision : " + revisionDetailViewModel.Revision + " by " + revisionDetailViewModel.Author + " on " + revisionDetailViewModel.Timestamp)
               .attr('class', 'commit-subtitle')
               .appendTo(title);

            $('<div></div>')
                .attr('class', 'commit-annotation-summary')
                .appendTo(block);

            if (revisionDetailViewModel.ApprovedBy) {
                $('<div></div>')
                    .text('This commit has been approved by: ' + revisionDetailViewModel.ApprovedBy)
                    .appendTo(actions);
            }
            else if (revisionDetailViewModel.Author != getUsername()) {
                $('<button></button>')
                    .attr('class', 'btn btn-success button-ok')
                    .text('Approve')
                    .on('click', { revision: revision }, markAsApproved)
                    .appendTo(actions);

                $('<button></button>')
                .attr('class', 'btn btn-success button-ok')
                .text('Add comment')
                .on('click', { revision: revision}, markAsReviewed)
                .appendTo(actions);
            }

            actions.appendTo(block);

            block.appendTo(insertPoint);

            showReviews(revisionDetailViewModel.Reviews, revision);

            // On success, 'data' contains a list of products.
            $.each(revisionDetailViewModel.RevisedFileDetails,
                function (key, revisedFileDetailsViewModel) {
                    showRevision(revisionDetailViewModel.Revision, revisedFileDetailsViewModel);
                });
        });
}

//
// show each revised file
//
function showRevision(revisionNumber, revisedFileDetailsViewModel) {

    var lineIndex,
        tableWrapper = $('<div></div>').attr('class', 'revision-table-wrapper'),
        table = $('<table></table>').attr('class', 'revision-file-table'),
        row;
    var lineDetailViewModel;
    var nbLines = revisedFileDetailsViewModel.LineDetails.length;
    var lineFragment;
    var codeCell;

    for (lineIndex = 0; lineIndex < nbLines; lineIndex++) {

        lineDetailViewModel = revisedFileDetailsViewModel.LineDetails[lineIndex];

        if ( ! IgnoreFirstLine( lineDetailViewModel ) ) {

            lineFragment = FileDiff_GetLineFragment(
                    lineDetailViewModel.ChangeState,
                    getRemovedLineNumber(lineDetailViewModel),
                    getAddedLineNumber(lineDetailViewModel),
                    getLineText(lineDetailViewModel));
 
            if (lineDetailViewModel.ChangeState !== 3) {

                lineFragment
                    .attr('data-revision', revisionNumber)
                    .attr('data-file', revisedFileDetailsViewModel.Filename)
                    .attr('data-line', lineDetailViewModel.LineId)
                    .on('click', addNewCommentBox); // TODO add style for cursor only in this case

                var codeCell = lineFragment.children[3];

                addComments(revisionNumber, revisedFileDetailsViewModel.Index, lineDetailViewModel, lineFragment);
            }

            table.append(lineFragment);
        }
    }

    var expandButton = $('<button></button>')
        .text('+')
        .attr('class','button-expand')
        .on('click', { revision: revisionNumber, filename: revisedFileDetailsViewModel.Filename }, expandFile);

    tableWrapper
        .append($('<div></div>')
        .attr('class', 'file-header')
        .append(expandButton)
        .append($('<span></span>').attr('class','label-change-type').text(revisedFileDetailsViewModel.ModText))
        .append($('<span></span>').text(revisedFileDetailsViewModel.Filename)));

    tableWrapper.append(table);
    
    $('#insertPoint').append(tableWrapper);
}

function IgnoreFirstLine(lineDetailViewModel) {
    return (lineDetailViewModel.RemovedLineNumber === 1 && lineDetailViewModel.ChangeState === 3);
}

function FileDiff_GetLineFragment(lineState, oldLineNumber, newLineNumber, text) {

    var changeStyle = getExtraStyleForState(lineState);

    lineFragment = $('<tr></tr>').attr('class', 'revision-line ' + changeStyle);

    if (lineState === 3) {
        $('<td></td>')
            .attr('colspan', '4')
            .attr('class', 'revision-line-text')
            .text('-- non-modified lines --')
            .appendTo(lineFragment);
    }
    else {
        $('<td></td>')
        .attr('class', 'revision-line-number ' + changeStyle)
        .text(oldLineNumber)
        .appendTo(lineFragment);

        $('<td></td>')
            .attr('class', 'revision-line-number ' + changeStyle)
            .text(newLineNumber)
            .appendTo(lineFragment);

        $('<td></td>')
            .attr('class', 'revision-line-state')
            .text(getChangedSymbol(lineState))
            .appendTo(lineFragment);

        $('<td></td>')
            .attr('class', 'revision-line-text')
            .text(text)
            .appendTo(lineFragment);
    }

    return lineFragment;
}

function expandFile(event) {
    var revision = event.data.revision;
    var filename = event.data.filename;

    var uri = 'api/commits/file/' + revision + '?filename=' + encodeURIComponent(filename);

    $.getJSON(uri)
        .done( function(data, textStatus, jqXHR)
        {
            var tbody = $(event.target).parent().parent().find('tbody').first();
            insertFile( tbody, data);
        });
}

function insertFile(tbody, lines) {
    var rows = tbody.children();
    var row;
    var lineIndex = 0;

    if ( rows.length == 0 )
        return;
    
    row = rows[0];

    while (row) {
        if (row.className.indexOf('state-break') >= 0) {

            // non modified place holder
            // peak line number of next row
            // insert before new lines from lineIndex to peekedlinenum
            // get next line (cannot be a place holder normally)
            // removeplaceholder

            var peekNode = row.nextSibling;

            if (peekNode === null) {
                row = null;
            }
            else {
                var td = peekNode.childNodes[0];
                var n = parseInt( td.innerHTML);

                for (; lineIndex < n - 1; lineIndex++) {

                    lineFragment = FileDiff_GetLineFragment(0, lineIndex + 1, lineIndex + 1, lines[lineIndex]);

                    lineFragment.insertBefore(peekNode);
                }
            }
            
            var del = row;
            row = peekNode;
            $(del).remove();
        }
        else {
            // move to next line
            row = row.nextSibling;
            lineIndex++;
        }
    }

    // add remaining lines
    while (lineIndex < lines.length) {
        lineFragment = FileDiff_GetLineFragment(0, lineIndex +1, lineIndex + 1, lines[lineIndex]);
        lineFragment.appendTo(tbody);
        lineIndex++;
    }
}

function showReviews(reviews, revision) {
    var nbReviews = reviews.length;
    var fragment;
    var insertPoint = $('#insertPoint');
    var i;
    
    var outer = $('<div></div>')
        .attr('Id', 'file-comments')
        .attr('data-revision', revision)
        .attr('class', 'review-line');

    insertPoint.append(outer);

    for( i = 0; i < nbReviews; i++ ) {
        fragment = createReviewBoxFragment(reviews[i]);
        if (reviews[i].ReplyToId !== null) {
            fragment.appendTo($(outer).find("div[data-idcomment='" + reviews[i].ReplyToId + "']:first"));
        }
        else {
            fragment.attr('class', fragment.attr('class') + ' comments-block-outer');
            fragment.appendTo(outer);
        }
    }
}

function createReviewBoxFragment(review) {
    var block = $('<div></div>')
        .attr('class', 'comments-block')
        .attr('data-idComment', review.Id );

    var header = $('<div></div>')
        .attr('class', 'comments-author');

    $('<span></span>').text(review.Author ).appendTo(header);

    commentReplyFragment(addNewReplyBox).appendTo(header);
    header.appendTo(block);

    $('<div></div>').attr('class', 'comments-text').text(review.Comment).appendTo(block);

    return block;
}

function markAsApproved(event) {

    var revision = event.data.revision;
    var approver = getUsername();

    var uri = 'api/commits/approve/' + revision + '/' + approver;
    $.post(uri)
        .done(function () {
            location.reload(true)
        }); // TODO
}

function markAsReviewed(event) {
    var revision = event.data.revision;
    var source = $(this);

    // find div where reviews are insert andd appendTo
    var reviewLine = $('#file-comments');

    var block = commentEditBoxFragment(postReviewComment, cancelReviewPost);
    block.appendTo(reviewLine);
    block.attr('class', block.attr('class') + ' comments-block-outer');
}

function cancelReviewPost(event) {
    $(this).parent().remove();
}

function postReviewComment(event) {
    var block = $(this).parent();
    var revision = block.parent().attr('data-revision');
    var textArea = block.find('textarea').first();
    var comment = textArea.val();
    var author = getUsername();
    var uri;
    var newBlock;

    if (!comment)
        return;

    uri = 'api/commits/comment/' + author + '/' + revision + "?comment=" + encodeURIComponent(comment);

    // we update screen optimistically before post

    newBlock = $('<div></div>').attr('class', 'comments-block');

    var authorBlock = $('<div></div>').attr('class', 'comments-author').text(author).appendTo(newBlock);

    commentReplyFragment(addNewReplyBox).appendTo(authorBlock);

    $('<div></div>').attr('class', 'comments-text').text(comment).appendTo(newBlock);

    if (block.parent().attr('class').indexOf('comments-block') == -1) {
        newBlock.attr('class', newBlock.attr('class') + ' comments-block-outer');
    }

    newBlock.insertAfter(block);
    block.remove();

    $.post(uri,
        function (data) {
            newBlock.attr('data-idcomment', data.Id); 
    });
}


//
// add comments associated with a line
//
function addComments(revisionNumber, fileId, line, appendToItem) {
    var i;
    var comments = line.Comments;

    for (i = 0; i < comments.length; i++) {
        addComment(comments[i], revisionNumber, fileId, line.LineId, appendToItem.children().last());
    }
}

//
// added a comment box
//
function addComment(comment, revisionNumber, fileId, lineId, appendToItem) {

    var isOuter = true;
    var appendPoint = appendToItem;
    if (comment.ReplyToId !== null) {
        isOuter = false;
        appendPoint = $(appendPoint).find("div[data-idcomment='" + comment.ReplyToId + "']:first");
    }

    var commentBox = newCommentFragment(comment.Id, comment.Author, comment.Comment, revisionNumber, fileId, lineId);

    if (isOuter)
        commentBox.attr('class', commentBox.attr('class') + ' comments-block-outer');

    commentBox.appendTo(appendPoint);
}

//
// create html fragment for the comment
//
function newCommentFragment(id, author, comment, revisionNumber, fileId, lineId ) {

    var block, header;

    block = $('<div></div>')
            .attr('class', 'comments-block')
            .attr('data-idComment', id);

    header = $('<div></div>')
        .attr('class', 'comments-author');

    $('<span></span>').text(author).appendTo(header);
    
    // anyone can reply to a comment
    commentReplyFragment(addNewReplyBox).appendTo(header);

    header.appendTo(block);

    $('<div></div>').attr('class', 'comments-text').text(comment).appendTo(block);

    return block;
}

//
// inserts a new comment box into the page
//
function addNewCommentBox(event) {

    event.stopPropagation();

    if (event.target.nodeName !== "TD")
        return;

    var line = $(this);
    var commentBox = newCommentBoxFragment(line.attr('data-revision'), line.attr('data-file'), line.attr('data-line'));

        commentBox.attr('class', commentBox.attr('class') + ' comments-block-outer');

    var codeCell = line.children().last();

    codeCell.append(commentBox);
}

function addNewReplyBox(event) {
    event.stopPropagation();
    var parentComment = $(this).parent().parent();
    var commentBox = commentEditBoxFragment( postReplyHandler, cancelReplyHandler);

    commentBox.attr('data-idcomment', parentComment.attr('data-idcomment'));
    commentBox.appendTo(parentComment);


}

function postReplyHandler(event) {
    event.stopPropagation();

    // get id of item to which re reply
    var parent = $(this).parent();
    var idComment = parent.attr('data-idcomment');
    textArea = parent.find('textarea').first();

    var author = getUsername();
    var comment = textArea.val();

    if (!comment) {
        return;
    }

    var uri = '/api/commits/reply/' + idComment + '/' + author + '?comment=' + encodeURIComponent(comment);

    // we update screen optimistically before post
    var block = newCommentFragment( -1, author, comment, null, null, null);

    var toRemove = textArea.parent();
    var insertPoint = toRemove.parent();

    insertPoint.append(block);
    toRemove.remove();

    $.post(uri, function (data) {
            block.attr('data-idcomment', data.Id); 
    });
}

function cancelReplyHandler(event) {
    event.stopPropagation();
    $(this).parent().remove();

}
    
//
// builds the html fragment for a new comment box
//
function newCommentBoxFragment(revisionNumber, fileId, lineId) {

    var row, cell, block, buttonBar;
    block = commentEditBoxFragment(postComment, cancelComment);
    return block;
}

function commentEditBoxFragment( postCommentHandler, cancelCommentHandler) {
    var block =
        $('<div></div>').attr('class', 'comments-block comments-block-new');

    var textArea =
        $('<textarea></textarea>')
        .attr('class', 'comments-box')
        .appendTo(block);

    var actionBar = $('<div></div>').attr('class','actionBar');

    $('<button></button>')
        .attr('class', 'btn btn-default btn-xs button-cancel')
        .text('Cancel')
        .on('click', cancelCommentHandler)
        .appendTo(block);

    $('<button></button>')
        .attr('class', 'btn btn-success btn-xs button-ok')
        .text('Post')
        .on('click', postCommentHandler)
        .appendTo(block);

    actionBar.appendTo(block);

    setTimeout(function () {
        block.find('textarea').focus();
    }, 20);

    return block;
}

function commentReplyFragment(commentReplyHandler) {

    // button is float right but is badly placed in or out of a div

    var block = $('<button></button>')
        .attr('class', 'btn btn-link btn-xs')
        .text('Reply')
        .on('click', commentReplyHandler);

    return block;        
}

//
// handles the cancellation of a new comment
//
function cancelComment() {
    $(this).parent().remove();
}

//
// handles the post commit button
//
function postComment() {

    var cell = $(this).parent().parent();
    var row = cell.parent();
    var textArea = cell.find('textarea').first();
    var revision = row.attr('data-revision');
    var fileIndex = row.attr('data-file');
    var line = row.attr('data-line');
    var author = getUsername();
    var comment = textArea.val();

    if (!comment) {
        return;
    }

    var uri = 'api/commits/comment/' + author + '/' + revision  + '/' + line + "?comment=" + encodeURIComponent(comment) + "&file=" + encodeURIComponent(fileIndex);

    // we update screen optimistically before post
    var block = newCommentFragment(-1, author, comment, revision, fileIndex, line);

    var toRemove = textArea.parent();
    var insertPoint = toRemove.parent();

    if (insertPoint.attr('class').indexOf('comments-block') == -1 ){
        block.attr('class', block.attr('class') + ' comments-block-outer');
    }

    insertPoint.append(block);
    toRemove.remove();

    $.post(uri,
        function (data) {
            block.attr('data-idcomment', data.Id); 
        });
}

//
// retrieves the text to display
//
function getLineText(data) {
    if (data.ChangeState === 3) {
        return "";
    }
    else
        return data.Text;
}

//
// get line number for an addition
//
function getAddedLineNumber(data) {
    if (data.ChangeState === 0 || data.ChangeState === 1) {
        return data.AddedLineNumber
    }
    else if (data.ChangeState == 3) {
        return "...";
    }

    return "";
}

//
// get line number of removed line
//
function getRemovedLineNumber(data) {
    if (data.ChangeState === 0 || data.ChangeState === 2) {
        return data.RemovedLineNumber
    }
    else if (data.ChangeState == 3) {
        return "...";
    }

    return "";
}

//
// get the css rule according to line state
//
function getExtraStyleForState(state) {
    if (state === 0) {
        return "state-none";
    }
    else if (state === 1) {
        return "state-added";
    }
    else if (state === 2) {
        return "state-removed";
    }

    return "state-break";
}

//
// get symbol (- or +) according to line state
//
function getChangedSymbol(state) {

    if (state === 1) {
        return "+";
    }
    else if (state === 2) {
        return "-";
    }
    return "";
}



// TOOD refactor
function createUser() {

    var userName = $('#userName').val();
    var password = $('#pwd').val();
    var confirm = $('#confirmPwd').val();

    if (!userName) {
        alert("usernamle required");
        return;
    }

    if (!password) {
        alert("password required");
        return;
    }

    if (password.length < 8) {
        alert("min 8 caracters please");
        return;
    }

    if (!confirm ) {
        alert("no confirm");
        return;
    }

    if (password !== confirm) {
        alert("confirmed not same");
        return;
    }

    $.post('api/users/create/' + userName + '?password=' + encodeURIComponent(password))
        .done(function () { alert(userName + ' created'); })
        .fail(function (xhr, textStatus, error) {
            alert(xhr.responseJSON.ExceptionMessage);
        });
}
