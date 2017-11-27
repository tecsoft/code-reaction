/*
** Tool box component to display review state and actions for currently viewed commit
*/

var commentNav= {
    name: 'comment-nav',
    props: ['Commit'],
    data: function () {
        return { indexPosComment:-1 }
    },
    template:
        '<div class="text-center">' +
            '<button id="PreviousComment" class="btn btn-primary" v-on:click="goPreviousCommit">' +
                '<i class="fa fa-chevron-up"></i>' +
            '</button>' +
            '<button id="NextComment" class="btn btn-primary" v-on:click="goNextCommit">' +
                '<i class="fa fa-chevron-down"></i>' +
            '</button>'+
        '</div>',

    methods: {
        goNextCommit: function () {

            var anchors = $('.comments-block-outer');

            if (anchors.length === 0) {
                return;
            }

            this.$data.indexPosComment++;

            if (this.$data.indexPosComment >= anchors.length) {
                this.$data.indexPosComment = 0; //Return to the first comment if the while is finish
            }

            var commentBlock = $(anchors[this.$data.indexPosComment]);

            var position = $(anchors[this.$data.indexPosComment]).position().top - 32;  // TODO could mosition in middle of top half of screen

            var origin = this.$el.ownerDocument.querySelector('.content');

            origin.scrollTop = position;

        },

        goPreviousCommit: function () {
            var anchors = $('.comments-block-outer');

            if (anchors.length === 0) {
                return;
            }

            this.$data.indexPosComment--;

            if (this.$data.indexPosComment <= 0) {
                this.$data.indexPosComment = anchors.length - 1; //Return to the last comment if the while is finish
            }

            var commentBlock = $(anchors[this.$data.indexPosComment]);

            var position = $(anchors[this.$data.indexPosComment]).position().top - 32;

            var origin = this.$el.ownerDocument.querySelector('.content');

            origin.scrollTop = position;
        },
    },
};