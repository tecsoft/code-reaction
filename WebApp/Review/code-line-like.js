/*
** component for displaying an existing comment and handling actions
*/
var codeLineLike = {
    props: ['Line'],
    name: 'code-line-like',
    computed: {
        getTextOthers: function () {

            var username = getUsername();
            var reorderLikes = this.Line.Likes.sort(
                 function (a, b) {
                     return a === username ? -1 : 1;
                 });

            return reorderLikes[0] + ", " + reorderLikes[1];

        },

        isLikedByUser: function () {
            for (var i = 0; i < this.Line.Likes.length; i++) {
                if (this.Line.Likes[i] === getUsername()) {
                    return true;
                }
            }

            return false;
        },
    },

    methods: {
        removeLike: function () {
            this.$emit("unlike-line");
        }

    },

    template:
        '<div class="comments-block comments-block-outer">' +
            '<div class="comments-author">' +
                
                '<span v-if="Line.Likes.length === 3"><strong>{{getTextOthers}}</strong> and <strong>1</strong> other liked this line</span>' +
                '<span v-else-if="Line.Likes.length > 3"><strong>{{getTextOthers}}</strong> and <strong>{{Line.Likes.length-2}}</strong> others liked this line</span>' +
                '<span v-else-if="Line.Likes.length === 2"><strong>{{Line.Likes[0]}}</strong> and <strong>{{Line.Likes[1]}}</strong> liked this line</span>' +
                '<span v-else-if="Line.Likes.length === 1"><strong>{{Line.Likes[0]}}</strong> liked this line</span>' +
                '<span v-if="isLikedByUser"><button class="btn btn-link btn-xs" v-on:click="removeLike"><i class="fa fa-remove"></i></button></span>' +
            '</div>' +
            '<div class="comments-text"></div></div>'
};