<template>
    <div v-cloak>
        <div class="content">
            <div class="detailsPanel2">
                <div id="insertPoint" v-if="!loading">
                    <b-list-group v-for="d in Commits" v-if="Commits.length > 0" class="separator">
                        <b-list-group-item :to="{path :'review', query: { revision: d.Revision}}">
                            <commit-details v-bind:commit="d" />
                            <div class="review-stats">
                                <b-badge v-if="d.ApprovedBy" class="label label-success"><i class="fa fa-check-circle"></i> {{d.ApprovedBy}}</b-badge>
                                <b-badge variant="primary"><i class="fa fa-eye"></i> {{d.NumberReviewers}}</b-badge>
                                <b-badge variant="success"><i class="fa fa-heart"></i> {{d.NumberLikes}}</b-badge>
                                <b-badge variant="danger"><i class="fa fa-comment"></i> {{d.NumberComments}}</b-badge>
                                <b-badge variant="warning"><i class="fa fa-comments"></i> {{d.NumberReplies}}</b-badge>
                            </div>
                        </b-list-group-item>
                    </b-list-group>
                    <div v-if="Commits.length === 0">
                        <span>Looks like your up to date</span>
                    </div>
                </div>
                <div v-else>Loading ...</div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import * as $ from 'jquery';
    import CommitDetails from '../review/commit-details.vue';
    import { Commit } from '../types/types';

    class ParameterList {
        max: number = 100;
        keyword: string = "";
        excludeApproved: boolean = true;
        include?: string = "";
    }

    @Component({ components: {CommitDetails}})
    export default class FollowUpsPage extends Vue {
        private Parameters: ParameterList = new ParameterList();
        private Commits: Array<Commit> = new Array<Commit>();
        private loading: boolean = true;
        Project: number = 1;

        refreshPage(this: FollowUpsPage): void {
            this.Parameters.include = this.$store.getters["account/username"];
            
            $.ajax({
                context: this,  // don't forget
                dataType: "json",
                url: "/api/commits",
                data: this.Parameters,
                crossDomain: true,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                .done(function (this: FollowUpsPage, data: any) {
                    this.loading = false;
                    this.Commits = data.Commits;
            })
                .fail(function (xhr: JQueryXHR) {
                    console.log(xhr.responseText);
            });
        }
        created() {
            this.refreshPage();
        }
    }
</script>

<style scoped>

    .review-stats {
        padding: 1em;
        float: left;
        width: 50%;
        text-align: right;
        font-size: 1.4em;
        margin-top: -1em
    }

    .review-stats .badge {
        color: #fff;
        margin-left: 5px;
    }

    .review-stats .badge-warning {
        background-color: #f0ad4e;
    }

    .review-stats .badge-primary {
        background-color: #337ab7;
    }

    .review-stats .badge-success {
        background-color: #5cb85c;
    }

    .review-stats .badge-danger {
        background-color: #d9534f;
    }

    .separator {
        margin-top: 4px;
    }
</style>