/*
** Component code-line_actions
*/
var lineActions = {
    name: 'code-line-actions',
    props : ['line'],
    data: function () { return {}; },
    template:
        '<span id="line-hover-panel" class="line-hover-in" v-on:removeLike>' +
            '<button v-if="canLike" class="btn btn-primary btn-xs"  v-on:click="addLike"><i class="fa fa-heart"></i></button>' +
            '<button v-if="canUnlike" class="btn btn-xs btn-primary disabled" ><i class="fa fa-heart"></i></button>' +
            '&nbsp;<button class="btn btn-primary btn-xs" v-on:click="addComment"><i class="fa fa-comment"></i></button>' +
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
            BUS.$emit('like-line', { Line: this.line } );
        },
        //removeLike: function() {
        //    BUS.$emit('unlike-line', { Line: this.line } );
        //}
    }
};