/*
**Page view
*/

var app = new Vue({
    el: "#vue",
    data: {
        Parameters: { max: 100, keyword: "", include: getUsername(), excludeApproved: true },
        Commits: {}
    },

    methods: {
        refreshPage: function (event) {

            $.getJSON("/api/commits", this.Parameters)
                .done(function (data) {
                    app.$data.Commits = data.Commits;
                });
        },
        openReview: function (revision) {
            window.location = "/Review/review.html?revision=" + revision;
        },
        timeAgo: function (aMoment) {
            if (aMoment)
                return moment(aMoment).fromNow();
        }

    },

    mounted: function () {
        this.refreshPage();
    }

});