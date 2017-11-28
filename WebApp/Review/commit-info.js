/*
** Component displaying commit information : Log, Author, ...
*/

var commitInfo = {
    name: 'commit-details',
    props: ['commit'],
    components: { 'commit-actions': commitActions },
    template:
        '<div class="commit-title2" >{{commit.Message}}' +
            '<div class="commit-subtitle">' +
                '<span>{{revisionText}} <strong>{{commit.Author}}</strong></span>' + // v-cloak doesn't hide hard coded text in templates
            '</div>' +
        '</div>',
    computed: {
        timeAgo: function () {
            if (this.commit.Timestamp) {
                return moment(this.commit.Timestamp).fromNow();
            }
        },

        revisionText: function () {
            if ( this.commit.Revision )
                return "Revision " + this.commit.Revision + " - committed " + this.timeAgo + " by ";
        },
    }
};