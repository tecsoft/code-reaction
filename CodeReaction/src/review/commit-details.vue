<template>
    <div class="commit-title">
        {{commit.Message}}
        <div class="commit-subtitle">
            <span>{{revisionText}} <strong>{{commit.Author}}</strong></span>
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment';
    import { Commit } from '../types/types';


    @Component({})
    export default class CommitDetails extends Vue {
        @Prop() commit!: Commit; 

        get revisionText() : string {
            if (this.commit.Revision) {
                return "Revision " + this.commit.Revision + " - committed " + this.timeAgo(this.commit.Timestamp) + " by ";   // to string literal concat
            }
            return "";
        }

        timeAgo(aMoment : Date): string {
            if (aMoment) {
                return moment(aMoment).fromNow();
            }

            return "";
        }
    }
</script>

<style scoped>

    .commit-title {
        font-size: 1em;
        font-weight: bolder;
        /*white-space: pre-wrap;*/
        width: 50%;
        float: left;
        color: black;
    }

    .commit-subtitle {
        font-size: 0.85em;
        color: #808080;
        font-weight: normal;
    }
</style>