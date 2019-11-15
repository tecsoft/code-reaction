<template>
    <div v-cloak>
        <b-form v-on:submit.prevent="" style="margin:1em">
            <b-form-group>
                <b-form-select v-if="projects.length > 1" style="max-width:400px;float:right; margin-right:0.5em;" v-model="project" v-bind:options="projects" v-on:change="change" />
                <b-form-input type="text"
                              v-model="Parameters.keyword"
                              placeholder="Keyword, Author"
                              style="max-width:400px;float:left; margin-right:0.5em;" />
                <b-button variant="primary" v-on:click="refreshPage" type="submit">
                    <i class="fa fa-search"></i>
                </b-button>
            </b-form-group>
            <b-form-group>
                <b-form-checkbox type="checkbox" inline
                                    v-model="Parameters.excludeApproved"
                                    id="inputExcludeApproved">
                    Exclude approved commits
                </b-form-checkbox>
                <b-form-checkbox type="checkbox" inline
                                    v-model="Parameters.excludeMine">
                    Exclude mine
                </b-form-checkbox>
            </b-form-group>
        </b-form>
        <div class="content" >
            <div class="detailsPanel2">
                <div id="insertPoint" v-if="!loading">
                    <b-list-group v-for="d in Commits" v-if="Commits.length > 0" class="separator">
                        <b-list-group-item :to="{path :'review', query: { revision: d.Revision}}">
                            <commit-details v-bind:commit="d" />
                            <div class="review-stats">
                                <b-badge v-if="d.ApprovedBy" variant="secondary"><i class="fa fa-check-circle"></i> {{d.ApprovedBy}}</b-badge>
                                <b-badge variant="primary"><i class="fa fa-eye"></i> {{d.NumberReviewers}}</b-badge>
                                <b-badge variant="success"><i class="fa fa-heart"></i> {{d.NumberLikes}}</b-badge>
                                <b-badge variant="danger"><i class="fa fa-comment"></i> {{d.NumberComments}}</b-badge>
                                <b-badge variant="warning"><i class="fa fa-comments"></i> {{d.NumberReplies}}</b-badge>
                            </div>
                            <!--<div class="commit-item-footer"></div>-->
                        </b-list-group-item>
                    </b-list-group>
                    <div v-if="Commits.length === 0">
                        <span>No results found</span>
                    </div>
                </div>
                <div v-else>
                    Loading ...
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import * as $ from 'jquery';
    import * as moment from 'moment';
    import CommitDetails from '../review/commit-details.vue';
    import { Commit } from '../types/types';
    import { ICommitSearchParameters } from '@/commits/store';

    class ParameterList implements ICommitSearchParameters {
        max: number = 100;
        keyword: string = "";
        excludeApproved: boolean = true;
        excludeMine: boolean = true;
        exclude?: string = "";
        project?: number = 1;
    }

    interface IProject {
        Id: number;
        Name: string;
    }

    interface IOption {
        value: number;
        text: string;
    }

    @Component({ components: {CommitDetails}})
    export default class CommitPage extends Vue {
        private Parameters: ParameterList = this.initParameters();
        private Commits: Array<Commit> = new Array<Commit>();
        private project: number = 1;
        private projects: Array<IOption> = new Array<IOption>();
        private loading: boolean = true;

        initParameters() : ParameterList {
            let parameters = this.$store.getters["commitCriteria/searchParameters"];
            if (parameters == null) {
                parameters = new ParameterList();
            }
            return parameters;
        }

        getProjectList() {

            this.projects = this.$store.getters["referenceData/projects"];

            if (this.projects.length == 0) {
                $.ajax({
                    context: this,
                    dataType: "json",
                    url: "/api/projects",

                    beforeSend: function (xhr: JQueryXHR) {
                        xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                    }
                })

                .done(function (this: CommitPage, data: Array<IProject>) {

                    this.projects = data.map(p => { return { value: p.Id, text: p.Name } });
                    this.$store.dispatch("referenceData/setProjects", this.projects);
                })
                .fail(function (xhr: JQueryXHR) {
                    console.log(xhr.responseText);
                });
            }
        }



        refreshPage(this: CommitPage): void {

            this.Parameters.project = this.project; // todo properly
            if (this.Parameters.excludeMine) {
                this.Parameters.exclude = this.$store.getters["account/username"];
            }
            else {
                this.Parameters.exclude = undefined;
            };

            this.$store.dispatch("commitCriteria/changeSearchParameters", this.Parameters);

            $.ajax({
                context: this,  // don't forget
                dataType: "json",
                url: "/api/commits",
                data: this.Parameters,
                crossDomain: true,
                beforeSend : function(xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                
                .done(function (this: CommitPage, data: any) {
                    this.loading = false;
                    this.Commits = data.Commits;
            })
                .fail(function (xhr: JQueryXHR) {
                    console.log(xhr.responseText);
                });

            
        }

        change(): void {
            
            this.$store.dispatch("commitCriteria/changeProject", this.project);
            this.refreshPage();
        }

        created() {
            this.project = this.$store.getters["commitCriteria/project"];
            this.getProjectList();
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
        color:#fff;
        margin-left:5px;
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
        margin-top:4px;
    }
    
    /*
    
    .commit-title {
        font-size: 1em;
        font-weight: bolder;
        white-space: pre-wrap;
        width: 50%;
        float: left;
        color: black;
    }
    .commit-item {
        padding: 0.25em;
    }
    .commit-item-footer {
        clear: both;
        border-bottom: 1px solid #CCC8C7;
        padding-bottom: 1em;
    }

    .commit-annotation-summary {
        padding: 1em;
        float: left;
        width: 50%;
        text-align: right;
    }

        .commit-annotation-summary .label {
            margin-left: 5px;
        }*/
</style>