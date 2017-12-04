/*
** component for displaying an existing comment and handling actions
*/
var codeLineLike = {
    props: ['Likes'],
    name: 'comment-line-like',
    computed: {
        getTextOthers: function () {

            var username = getUsername();
            var reorderLikes = this.Likes.sort(
                 function (a, b) {
                     return a === username ? -1 : 1;
                 });

            return reorderLikes[0] + ", " + reorderLikes[1];

        }
    },

    template:
        '<div class="comments-block comments-block-outer">' +
            '<div class="comments-author">' +
                '<span v-if="Likes.length === 3"><strong>{{getTextOthers}}</strong> and <strong>1</strong> other liked this line</span>' +
                '<span v-else-if="Likes.length > 3"><strong>{{getTextOthers}}</strong> and <strong>{{Likes.length-2}}</strong> others liked this line</span>' +
                '<span v-else-if="Likes.length === 2"><strong>{{Likes[0]}}</strong> and <strong>{{Likes[1]}}</strong> liked this line</span>' +
                '<span v-else-if="Likes.length === 1"><strong>{{Likes[0]}}</strong> liked this line</span>' +
            '</div>' +
            '<div class="comments-text"></div></div>'
};