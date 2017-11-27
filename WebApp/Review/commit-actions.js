/*
** Tool box component to display review state and actions for currently viewed commit
*/

var commitActions = {
    name: 'commit-actions',
    props: ['Commit'],
    template:
        '<div class="commit-actions2">' +
            '<span v-if="Commit.ApprovedBy" class="label label-success"><i class="fa fa-check-circle"></i> Approved by {{Commit.ApprovedBy}}</span>' +
            '<div v-else>' +
                '<button v-if="Commit.Author != getUsername()"  class="btn btn-success button-ok" v-on:click="approveCommit">Approve</button>' +
                '<button class="btn btn-success button-ok" v-on:click="newComment">Add comment</button>' +
            '</div>' +
        '</div>',
    methods: {
        approveCommit: function () {
            BUS.$emit('approve-commit', this.Commit);
        },
        newComment: function () {
            this.$emit("add-comment");
        },
    },
};
