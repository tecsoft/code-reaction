/*
** Component for creating a new comment or reply
** Events
**   new-comment-cancelled : editor closed without action
**   posted-new-comment : comment posted
*/

var newComment = {
    props: ['show'],
    name: 'new-comment-block',
    data: function () {
        return { Id: -1, ReplyToId: '', Message: '' };
    },
    template:
        '<div v-if="show" class="comments-block comments-block-new">' +
            '<textarea class="comments-box "v-model="Message"></textarea>' +
                '<button class="btn btn-default btn-xs button-cancel" v-on:click="newCommentCancelled">Cancel</button>' +
                '<button class="btn btn-success btn-xs button-ok" v-on:click="postNewComment">Post</button>' +
                '<div class="actionBar"></div>' +
        '</div>',

    methods: {

        newCommentCancelled: function () {
            this.Message = null;
            this.$emit("new-comment-cancelled");
        },

        postNewComment: function () {
            if (this.Message) {
                this.$emit("posted-new-comment", this.Message);
            }
        },
    }
};

/*
** component for displaying an existing comment and handling actions
*/
var comment = {
    props: ['Comment', 'IsInner'],
    name: 'comment-block',

    data() { return { showEditor: false }; },

    methods: {
        addReply: function () {
            this.showEditor = true;
        },

        postedReply: function (message) {

            BUS.$emit("posted-reply", { Comment: this.Comment, Message: message });
            this.showEditor = false;

        },

        cancelReply: function () {
            this.showEditor = false;
        }
    },
    computed : {
        getClassForOuterBlock : function() {
            if (!this.IsInner) {
                return "comments-block-outer";
            }
        },
    },
    components: { 'new-comment-block': newComment },
    template:
        '<div v-if="Comment.Id >= 0" class="comments-block" v-bind:class="getClassForOuterBlock" >' +
            '<div class="comments-author">' +
                '<span>{{Comment.Author}}</span><button class="btn btn-link btn-xs" v-on:click="addReply()">Reply</button>' +
                '<span class="comments-when"></span>' +
            '</div>' +
            '<div class="comments-text">{{Comment.Text}}</div>' +
            '<new-comment-block v-bind:show="showEditor" v-on:posted-new-comment="postedReply" v-on:new-comment-cancelled="cancelReply"/>' +
            '<comment-block v-for="reply in Comment.Replies" v-bind:Comment="reply" v-bind:IsInner="true" v-on:posted-new-comment="postedReply" ></comment-block>' +
        '</div>'
};