/*
** Component code-line_actions
*/
var lineActions = {
    name: 'code-line-actions',
    props : ['line'],
    data: function () { return {}; },
    template:
        '<span id="line-hover-panel" class="line-hover-in">' +
            '<button class="btn btn-primary btn-xs" v-on:click="addComment"><i class="fa fa-comment-o"></i></button>' +
            '&nbsp;<button class="btn btn-primary btn-xs" v-if="canLike" v-on:click="addLike"><i class="fa fa-heart-o"></i></button>' +
            '<button class="btn btn-primary btn-xs" v-if="canUnlike" v-on:click="removeLike"><i class="fa fa-heart"></i></button>' +
        '</span>',
    computed : {
        canLike : function() {
            return this.line.Author !== getUsername() && this.canUnlike === false;
        },
        canUnlike: function () {
            for (var i = 0; i < this.line.Likes.length; i++) {
                if (this.line.Likes[i] === getUsername()) {
                    return true;
                }
            }
            return false;
        }
    },
    methods: {
        addComment: function () {
            this.$emit('add-comment');
        },
        addLike: function () {
            this.$emit('like-line');
        },
        removeLike: function() {
            this.$emit('unlike-line');
        }
    }
};