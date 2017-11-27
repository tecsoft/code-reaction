/*
** Component code-line_actions
*/
var lineActions = {
    name: 'code-line-actions',
    data: function () { return {}; },
    template:
        '<span id="line-hover-panel" class="line-hover-in">' +
            '<button class="btn btn-primary btn-xs" v-on:click="addComment">+</button>' +
        '</span>',
    methods: {
        addComment: function () {
            this.$emit('add-comment');
        }
    }
};