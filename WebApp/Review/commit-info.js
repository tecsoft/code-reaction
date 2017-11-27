/*
** Component displaying commit information : Log, Author, ...
*/

var commitInfo = {
    name: 'commit-details',
    props: ['commit'],
    components: { 'commit-actions': commitActions },
    template:
        '<div class="commit-title2 ">{{commit.Message}}' +
            '<div class="commit-subtitle">' +
                '<span>Revision: {{commit.Revision}} {{timeAgo}} by {{commit.Author}}</span>' +
            '</div>' +
        '</div>',
    computed: {
        timeAgo: function () {
            if (this.commit.Timestamp) {
                return moment(this.commit.Timestamp).fromNow();
            }
        }
    }
};