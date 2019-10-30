<template>
    <div class="commit-actions" v-cloak>
        <div v-if="commit.ApprovedBy">
            <b-badge class="label label-success"><i class="fa fa-check-circle"></i>Approved by {{commit.ApprovedBy}}</b-badge>
        </div>
        <div v-else>
            <b-button variant="success" squared
                    v-if="!isAuthor" 
                    v-on:click="approveCommit">Approve</b-button>
            <b-button variant="success" squared
                    v-on:click="newComment">Add comment</b-button>
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { Commit } from '../types/types';

    @Component({})
    export default class CommitActions extends Vue {
        @Prop() commit!: Commit;

        approveCommit() : void {
            this.$emit('approve-commit', this.commit);
        }

        newComment() : void {
            this.$emit("add-comment");
        }

        get isAuthor(): boolean {
            return this.commit.Author === this.$store.getters["account/username"];
        }
    }
</script>

<style scoped>
    .commit-actions {
        padding: 1em;
        float: left;
        width: 50%;
        text-align: right;
    }

    .badge {
        font-size:1.1em;
    }
</style>