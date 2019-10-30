<template>
    <div v-if="show" class="comments-block comments-block-new">
        <b-form-textarea autofocus class="comments-box" v-model="message"></b-form-textarea>
        <b-button variant="outline-secondary" size="sm" squared v-on:click="newCommentCancelled">Cancel</b-button>
        <b-button variant="success"  size="sm" squared v-on:click="postNewComment">Post</b-button>
    <div class="actionBar"></div>
</div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
   // import { Commit, Comment } from '../types/types';

    @Component
    export default class NewCommentBlock extends Vue {
        @Prop() show ?: boolean = false;

        //id : number = -1;
        //replyToId : number = -1;
        message: string = "";

        newCommentCancelled() : void {
            this.message = "";
            this.$emit("new-comment-cancelled");
        }

        postNewComment(): void {
            if (this.message) {
                this.$emit("posted-new-comment", { message: this.message });
                this.message = "";
            }
        }
    }

</script>

<style scoped>
    .comments-block-new > .btn {
        float:right;
        margin-right:1em;
        margin-top:1em;
    }
</style>