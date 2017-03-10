
//
// Load the review page for the revision specified in the parameter
//
var revisionModel = null;

function loadReview() {

    var revision = getParameterByName('revision');
    var uri = 'api/commits/revision/' + revision;

    $.getJSON(uri)
        .done(function (revisionDetailViewModel) {

            revisionModel = revisionDetailViewModel;

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
                $('<button class="btn btn-success button-ok"><i class="fa fa-check"></i>  Approve</button>')
                    .on('click', { revision: revision }, markAsApproved)
                    .appendTo(actions);

                $('<button class="btn btn-primary button-ok"><i class="fa fa-edit"></i></button>')
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

            loadLikes(revision);
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
                    .attr('data-line', lineDetailViewModel.LineId);
                    // TODO add style for cursor only in this case

                var codeCell = lineFragment.children[3];

                addComments(revisionNumber, lineDetailViewModel, lineFragment);
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
    return lineDetailViewModel.RemovedLineNumber === 1 && lineDetailViewModel.ChangeState === 3;
}

function FileDiff_GetLineFragment(lineState, oldLineNumber, newLineNumber, text) {

    var changeStyle = getExtraStyleForState(lineState);

    lineFragment = $('<tr class="revision-line ' + changeStyle + '"></tr>');

    if (lineState === 3) {
        $('<td colspan="4" class="revision-line-text">-- non-modified lines --</td>').appendTo(lineFragment);
    }
    else {
        $('<td class="revision-line-number ' + changeStyle + '">' + oldLineNumber + '</td>').appendTo(lineFragment);
        $('<td class="revision-line-number ' + changeStyle + '">' + newLineNumber + '</td>').appendTo(lineFragment);

        var htmlLineCommand = '<td class="revision-line-state">' + getChangedSymbol(lineState) +
                                '<span class="btn btn-primary btn-xs btn-comment-line"><i class="fa fa-edit"/></span>';
                                if (revisionModel.Author != getUsername()) {
                                    htmlLineCommand += '<span class="btn btn-success btn-xs btn-like-line"><i class="fa fa-thumbs-up"/></span>';
                                }
                                htmlLineCommand += '</td>';

        $(htmlLineCommand).appendTo(lineFragment);
        $('<td class="revision-line-text"></td>').text(text).appendTo(lineFragment); //.text else is interpreter by browser as html
    }

    return lineFragment;
}

$("body").on("click", ".btn-comment-line", function () {
    addNewCommentBox($(this));
});

$("body").on("click", ".btn-like-line", function () {
    postLike($(this));
});

$("body").on("click", ".btn-unlike-line", function () {
    deleteLike($(this));
});

function postLike(elem) {
    var line = elem.closest(".revision-line");
    var lineId = line.attr("data-line");
    var revision = line.attr("data-revision");
    var user = getUsername();
    var tdText = line.find(".revision-line-text");

    uri = '/api/commits/like/' + user + '/' + revision + '/' + lineId;

    $.post(uri,
    function (data) {
        addLike(elem, tdText, data.Id, getUsername());
    });
}

function deleteLike(elem) {

    var likeMsg = elem.closest(".like-line-message");

    uri = '/api/commits/like/' + likeMsg.attr("data-idlike");

    $.ajax({
        url: uri,
        type: 'delete',
        contentType: 'application/json',
        success: function (result) {

            likeMsg.closest(".revision-line").find(".btn-like-line").removeClass("is-liked");
            likeMsg.remove();
        }
    });
}

function addLike(btn, texteBlock, likeId, author) {

    var htmlLikeThis = '<div class="like-line-message" data-idlike="' + likeId + '" data-author="' + author + '">WOOOOW ' + author + ' <i class="fa fa-heart" /> this.';

    if (author == getUsername()) {
        htmlLikeThis += '<span class="btn btn-xs btn-unlike-line"><i class="fa fa-remove"/></span>';
    }
    htmlLikeThis += '</div>';

    texteBlock.append(htmlLikeThis);

    if (author == getUsername()) {
        btn.addClass("is-liked");
    }
}

function loadLikes(revision) {
    uri = '/api/commits/likes/' + revision;

    $.getJSON(uri)
      .done(function (likes) {
          for (var i = 0; i < likes.length; i++) {
              var like = likes[i];
              var line = $("[data-line='" + like.LineId + "']");

              addLike(line.find(".btn-like-line"), line.find(".revision-line-text"), like.Id, like.User);
          }
    });
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

    for (i = 0; i < nbReviews; i++) {
        var review = reviews[i];
        fragment = newCommentFragment(review.Id, review.Author, review.Comment);
        if (review.ReplyToId !== null) {
            fragment.appendTo($(outer).find("div[data-idcomment='" + review.ReplyToId + "']:first"));
        }
        else {
            fragment.attr('class', fragment.attr('class') + ' comments-block-outer');
            fragment.appendTo(outer);
        }
    }
}

function markAsApproved(event) {

    var revision = event.data.revision;
    var approver = getUsername();

    var uri = 'api/commits/approve/' + revision + '/' + approver;
    $.post(uri)
        .done(function () {
            location.reload(true);
        }); // TODO
}

function markAsReviewed(event) {
    var revision = event.data.revision;
    var source = $(this);

    // find div where reviews are insert andd appendTo
    var reviewLine = $('#file-comments');

    var block = commentEditBoxFragment(postReviewComment);
    block.appendTo(reviewLine);
    block.attr('class', block.attr('class') + ' comments-block-outer');
}

function postReviewComment(event) {
    var block = $(this).parent();
    var revision = block.parent().attr('data-revision');
    var textArea = block.find('textarea').first();
    var comment = textArea.val();
    var author = getUsername();
    var uri;
    var newBlock;

    if (!comment || comment.trim().length === 0) {
        textArea.focus();
        return;
    }

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
function addComments(revisionNumber, line, appendToItem) {
    var comments = line.Comments;

    for (var i = 0; i < comments.length; i++) {
        addComment(comments[i], appendToItem.find(".revision-line-text"));
    }
}

//
// added a comment box
//
function addComment(comment, appendToItem) {

    var isOuter = true;
    var appendPoint = appendToItem;
    if (comment.ReplyToId !== null) {
        isOuter = false;
        appendPoint = $(appendPoint).find("div[data-idcomment='" + comment.ReplyToId + "']:first");
    }

    var commentBox = newCommentFragment(comment.Id, comment.Author, comment.Comment);

    if (isOuter)
        commentBox.attr('class', commentBox.attr('class') + ' comments-block-outer');

    commentBox.appendTo(appendPoint);
}

//
// create html fragment for the comment
//
function newCommentFragment(id, author, comment ) {

    var block = $('<div class="comments-block" data-idComment="' + id + '">' +
                        '<div class="comments-author">' +
                            '<span>' + author + '</span>' +
                            btnReply() +
                        '</div>' +
                        '<div class="comments-text">' + comment + '</div>' +
                    '</div>');
    return block;
}

//
// inserts a new comment box into the page
//
function addNewCommentBox(elem) {

    var line = elem.closest(".revision-line");

    var commentBox = newCommentBoxFragment(line.attr('data-revision'), line.attr('data-file'), line.attr('data-line'));

    commentBox.attr('class', commentBox.attr('class') + ' comments-block-outer');

    var codeCell = line.find(".revision-line-text");

    codeCell.append(commentBox);
}

function addNewReplyBox(event) {
    event.stopPropagation();
    var commentblock = $(this).closest(".comments-block");
    var commentBox = commentEditBoxFragment();

    commentBox.attr('data-idcomment', commentblock.attr('data-idcomment'));
    commentBox.appendTo(commentblock);
}
    
//
// builds the html fragment for a new comment box
//
function newCommentBoxFragment(revisionNumber, fileId, lineId) {

    var row, cell, block, buttonBar;
    block = commentEditBoxFragment();
    return block;
}

function commentEditBoxFragment() {

    var block = $('<div class="comments-block comments-block-new">' +
                    '<textarea class="comments-box"></textarea>' +
                    '<div class="action-bar">' +
                        '<button class="btn btn-success btn-xs button-ok btn-post-comment">Post</button>' +
                        '<button class="btn btn-link btn-xs button-cancel btn-cancel-comment">Cancel</button>' +
                    '</div>' +
                '</div>');

    setTimeout(function () {
        block.find('textarea').focus();
    }, 20);

    return block;
}

function btnReply() {

    // button is float right but is badly placed in or out of a div
    return '<div class="btn-comment-action">' +
                '<button class="btn btn-primary btn-xs btn-reply">' +
                    '<i class="fa fa-reply"></i>' +
                '</button>' +
            '</div>';
}

// Cancellation of a new comment event
$("body").on("click", ".btn-cancel-comment", function () {
    $(this).closest(".comments-block").remove();
});

$("body").on("click", ".btn-reply", addNewReplyBox);

// Post comment event
$("body").on("click", ".btn-post-comment", function () {

    var cell = $(this).closest(".comments-block");
    var row = cell.closest("[data-revision]");
    var textArea = cell.find('textarea');
    var revision = row.attr('data-revision');
    var fileIndex = row.attr('data-file');
    var line = row.attr('data-line');
    var author = getUsername();
    var comment = textArea.val();

    var idComment = cell.attr('data-idcomment');

    if (!comment || comment.trim().length === 0) {
        textArea.focus();
        return;
    }

    var block, uri;

    if (idComment) {

        uri = '/api/commits/reply/' + idComment + '/' + author + '?comment=' + encodeURIComponent(comment);

        // we update screen optimistically before post
        block = newCommentFragment(-1, author, comment);

        var toRemove = cell;
        var insertPoint = toRemove.parent();
    }
    else {

        uri = 'api/commits/comment/' + author + '/' + revision;

        //no line if is heading revision
        if (line)
            uri += '/' + line;

        uri += '?comment=' + encodeURIComponent(comment);

        //no file index if is heading revision
        if (fileIndex)
            uri += '&file=' + encodeURIComponent(fileIndex);


        // we update screen optimistically before post
        block = newCommentFragment(-1, author, comment);

        var toRemove = cell;
        var insertPoint = toRemove.parent();

        if (insertPoint.attr('class').indexOf('comments-block') == -1) {
            block.attr('class', block.attr('class') + ' comments-block-outer');
        }
    }

    insertPoint.append(block);
    toRemove.remove();

    $.post(uri,
    function (data) {
        block.attr('data-idcomment', data.Id);
    });
});

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
        return data.AddedLineNumber;
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
        return data.RemovedLineNumber;
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


var indexPosComment = -1;
var currentPos = 0;

$(window).keydown(function (e) {
    switch (e.keyCode) {
        case 38:
            PreviousComment();
            return;
        case 40: // down arrow key
            NextComment();
            return;
    }
});

$("#NextComment").on("click", function () {
    NextComment();
});

$("#PreviousComment").on("click", function () {
    PreviousComment();
});

function NextComment() {

    indexPosComment++;

    var anchors = $('.comments-block-outer');

    if (indexPosComment >= anchors.length) {
        indexPosComment = -1; //Return to the first comment if the while is finish
        currentPos = 0;
    }
    else {
        var posNextComment = $(anchors[indexPosComment]).position().top;
        currentPos += posNextComment;
    }

    $('#detailPanel2').animate({
        scrollTop: currentPos
    }, 500);
}

function PreviousComment() {
    indexPosComment--;

    var anchors = $('.comments-block-outer');

    if (indexPosComment < 0) {
        indexPosComment = anchors.length; //Return to the first comment if the while is finish
        currentPos = 0;
    }
    else {
        var posNextComment = $(anchors[indexPosComment]).position().top;
        currentPos += posNextComment;
    }

    $('#detailPanel2').animate({
        scrollTop: currentPos
    }, 500);
}