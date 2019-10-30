<template>
    <div v-cloak>
        <div class="content">
            <div id="detailPanel2">
                <div id="insertPoint">

                    <div class="commit-item" v-if="!loading">
                        <commit-details v-bind:commit="commit" />
                        <commit-actions v-bind:commit="commit"
                                        v-on:add-comment="openCommentDialog"
                                        v-on:approve-commit="approveCommit"
                                        />
                    </div>

                    <div class="review-line">
                        <comment-block v-for="commitComment in commit.CommitComments"
                                       v-bind:comment="commitComment" 
                                       v-on:posted-reply="postReply"
                                       v-on:posted-ok="postOK"/>
                        <new-comment-block v-bind:show="showEditor"
                                           v-on:posted-new-comment="postedNewComment"
                                           v-on:new-comment-cancelled="cancelledNewComment"
                                           
                                           v-on:posted-reply="postReply"/>
                    </div>

                    <div class="revision-table-wrapper">
                        <file-block v-for="fileReview in commit.Files"
                                    v-bind:file="fileReview"
                                    v-on:new-code-comment="postCodeComment"
                                    v-on:like-line="likeLine"
                                    v-on:unlike-line="unLikeLine"
                                    v-on:posted-ok="postOK"
                                    v-on:posted-reply="postReply"/>
                    </div>
                </div>
            </div>
        </div>
        <div class="footer">
            <comment-nav />
        </div>

    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from 'vue-property-decorator';
    import moment from 'moment';
    import { Commit } from '../types/types';
    import * as $ from 'jquery';
    import CommitDetails from './commit-details.vue';
    import CommitActions from './commit-actions.vue';
    import CommentBlock from './comment-block.vue';
    import NewCommentBlock from './new-comment-block.vue';
    import FileBlock from './file-block.vue';
    import CommentNav from './comment-nav.vue';

    @Component({
        components: { CommitDetails, CommitActions, CommentBlock, NewCommentBlock , FileBlock, CommentNav}
    })
    export default class ReviewPage extends Vue {

        revision?: any;
        showEditor: boolean = false;
        commit: any = {};
        loading: boolean = true;

        @Watch('$route', { immediate: true, deep: true })
        onUrlChange(newVal: any) {
            this.loadReview();
        };

        loadReview() {
            this.revision = this.$router.currentRoute.query.revision;

            $.ajax({
                context: this,  // don't forget
                dataType: "json",
                url: "api/review/revision/" + this.revision,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                .done(function (this: ReviewPage, data: any) {
                    this.loading = false;
                    this.commit = data;
            })
                .fail(function (xhr: JQueryXHR) {
                console.log(xhr.responseText);
            });
        }

        timeAgo(aMoment: Date) : any {
            if (aMoment)
                return moment(aMoment).fromNow();

            return "";
        }

        openCommentDialog() : void {
            this.showEditor = true;
        }

        closeCommentDialog() {
            this.showEditor = false;
        }

        postedNewComment(event: any): void {
            var username = this.$store.getters["account/username"];
            var comment = { Id: 1, Author: username, Text: event.message, Replies: [], Timestamp: moment().utc() };
            this.closeCommentDialog();

            this.commit.CommitComments.push(comment);
            var uri = '/api/review/comment/' + comment.Author + '/' + this.commit.Revision + "?comment=" + encodeURIComponent(comment.Text);
            $.ajax({
                method: 'POST',
                context: this,  // don't forget
                dataType: "json",
                url: uri,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                .done(function (this: any, data:any) {
                    var temp = this.commit.CommitComments[this.commit.CommitComments.length - 1];
                    temp.Id = data;
                    this.commit.CommitComments[this.commit.CommitComments.length - 1] = temp;
                });
        }

        cancelledNewComment() : void {
            this.closeCommentDialog();
        }

        approveCommit() : void {
            var approver = this.$store.getters["account/username"];

            var uri = '/api/review/approve/' + this.commit.Revision + '/' + approver;
            $.ajax({
                method: 'POST',
                context: this,  // don't forget
                //dataType: "json",
                url: uri,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
            .done(function (this: ReviewPage, data:any) {
                this.commit.ApprovedBy = approver;
            });
        }


        postCodeComment(event:any): void {

            var line = event.Line;

            var poster = this.$store.getters["account/username"];

            line.Comments.push(
                { Id: 1, File: line.File, LineId: line.Id, Replies: [], Text: event.Text, Revision: line.Revision, Author: poster, Timestamp: moment.utc() }
            );

            var uri = '/api/review/comment/' + poster + '/' + line.Revision + '/' + line.Id + "?comment=" + encodeURIComponent(event.Text) + "&file=" + encodeURIComponent(line.File);

            var temp = { Id: 1, Author: poster, Text: event.Text, Replies: [], Timestamp: moment.utc() };

            this.showEditor = false;

            $.ajax({
                method: 'POST',
                context: this,  // don't forget
                dataType: "json",
                url: uri,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                .done(function (data) {
                    var temp = line.Comments[line.Comments.length - 1];
                    temp.Id = data;
                    line.Comments[line.Comments.length - 1] = temp;
                });
        }

        postReply(event: any): void {
            var comment = event.comment;
            var username = this.$store.getters["account/username"];

            comment.Replies.push(
                { Id: 1, Text: event.message, Replies: [], ReplyToId: comment.Id, Timestamp: moment().utc(), Author: username }
            );

            var uri = '/api/review/reply/' + event.comment.Id + '/' + username + '?comment=' + encodeURIComponent(event.message);

            $.ajax({
                method: 'POST',
                context: this,  // don't forget
                dataType: "json",
                url: uri,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
            .done( function (data) {
                var temp = comment.Replies[comment.Replies.length - 1];
                temp.Id = data;
                comment.Replies[comment.Replies.length - 1] = temp;
            });
        }
        
        postOK(event: any): void {
            var comment = event.comment;
            var text = "OK, thanks!";
            var username = this.$store.getters["account/username"];
            comment.Replies.push(
                { Id: 1, Text: text, Replies: [], ReplyToId: comment.Id, Timestamp: moment().utc(), Author: username }
            );

            
            var uri = '/api/review/reply/' + comment.Id + '/' + username + '?comment=' + encodeURIComponent(text);

            $.ajax({
                method: 'POST',
                context: this,  // don't forget
                dataType: "json",
                url: uri,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
            .done(function (data) {
                var temp = comment.Replies[comment.Replies.length - 1];
                temp.Id = data;
                comment.Replies[comment.Replies.length - 1] = temp;
            });
        }

        likeLine(event: any) {
            var line = event.Line;

            var poster = this.$store.getters['account/username'];
            line.Likes.push(poster);

            var uri = '/api/review/like/' + poster + '/' + line.Revision + '/' + line.Id + "?&file=" + encodeURIComponent(line.File);

            $.ajax({
                method: 'POST',
                context: this,  // don't forget
                dataType: "json",
                url: uri,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                .done(function (data) {
                });
        }
        unLikeLine(event: any): void {
            
            var line = event;
            var index = -1;
            var username = this.$store.getters["account/username"];
            for (var i = 0; i < line.Likes.length && index === -1; i++) {
                if (line.Likes[i] === username) {
                    index = i;
                }
            }

            if (index !== -1) {
                line.Likes.splice(index, 1);
                var uri = '/api/review/unlike/' + username + '/' + line.Revision + '/' + line.Id + "?&file=" + encodeURIComponent(line.File);
                $.ajax({
                    method: 'POST',
                    context: this,  // don't forget
                    dataType: "json",
                    url: uri,
                    beforeSend: function (xhr: JQueryXHR) {
                        xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                    }
                })
                    .done(function (data) {
                    });
            }
        }

        created() {
            this.loadReview();
        }
    }
</script>

<style scoped>
    #detailPanel2 {
        background-color: #ffffff;
        padding-left: 2%;
        padding-right: 2%;
        overflow-y: auto;
        overflow-x: auto;
        position: relative;
    }
    
    .revision-table-wrapper {
        overflow-x: auto;
        overflow-y: hidden;
        padding: 0;
        clear: both;
        margin-bottom: .5em;
    }

    .review-line {
        border-top: 1px solid #cac4c4;
        padding-left: 4em;
        padding-top: .5em;
        clear: both;
    }

    .footer {
        margin-top: .2em;
        margin-bottom: .2em;
    }

</style>