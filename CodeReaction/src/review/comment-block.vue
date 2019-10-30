<template>
    <div v-if="comment.Id >= 0" class="comments-block" v-bind:class="getClassForOuterBlock">
        <div class="comments-author">
            <span><strong>{{comment.Author}}</strong> commented {{commentTime}}</span>
            <button v-if="canOk" class="btn btn-link btn-xs" v-on:click="addOk"><i class="fa fa-check-circle"></i> </button>
            <button class="btn btn-link btn-xs" v-on:click="addReply"><strong><i class="fa fa-reply"></i></strong> </button>
        </div>
        <div class="comments-text"><span>{{comment.Text}}</span></div>
        <new-comment-block v-bind:show="showEditor" 
                           v-on:posted-new-comment="postedReply"
                           v-on:new-comment-cancelled="cancelReply" />
        <comment-block v-for="reply in comment.Replies" 
                       v-bind:comment="reply" 
                       v-bind:isInner="true"
                       v-on="$listeners"/>  <!-- since we are a recursive comment don't listen to own events -->
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { Commit, Comment } from '../types/types';
    import NewCommentBlock from './new-comment-block.vue';
    import moment from 'moment';

    @Component({
        name : "comment-block",
        components: {NewCommentBlock}
    })
    export default class CommentsBlock extends Vue {
        @Prop() comment !: Comment;
        @Prop() isInner !: boolean;

        showEditor: boolean = false;
        message: string = "";

        addReply(): void {
            this.showEditor = true;
        }

        addOk(): void {
            this.$emit("posted-ok", { comment: this.comment });
        }

        postedReply(event: any): void {
            this.$emit("posted-reply", { comment: this.comment, message: event.message });
            this.showEditor = false;
            this.message = "";
        }

        cancelReply(): void {
            this.showEditor = false;
            this.message = "";
        }

        get getClassForOuterBlock(): string {
            if (!this.isInner) {
                return "comments-block-outer";
            }

            return "";
        }

        get canOk(): boolean {
            return this.comment.Author !== this.$store.getters["account/username"];
        }

        get commentTime(): string {
            if (this.comment.Timestamp) {
                return moment(this.comment.Timestamp).fromNow();
            }
            return "";
        }
        


    }
</script>

<style >
    .comments-box {
        resize: vertical;
        width: 100%;
        min-height: 5em;
        display: block;
        margin-bottom: .8em;
        background-color: #fff;
        border: 1px solid #cac4c4;
    }

    .comments-block {
        margin-top: .5em;
        margin-left: 2em;
        background-color: #fff;
        border: none;
        font-family: "Segoe UI", Helvetica Neue, Helvetica, Arial, sans-serif;
        margin-bottom: .5em;
    }

    .revision-line-text > .comments-block {
        font-size: 1.15em;
        margin-left: 8em;
        background-color: #fff;
    }

    .comments-block-outer {
        max-width: 75%;
        border: 1px solid #cac4c4;
    }

    .comments-block-new {
        border: none;
        background-color: #fff;
    }

    .comments-author, .comments-author-like {
        font-size: .95em;
        /*font-weight: bolder;*/
        color: #808080;
        background-color: #fff;
        padding-left: .2em;
        background-color: #eeeeee;
        border: 1px solid #CCC8C7;
        border-right: none;
    }

    .comments-author > .btn {
        padding:0;
    }
    
    .comments-author-like {
        font-size: .7em;
    }

    .comments-buttons {
        font-weight: normal;
        float: right;
        padding-right: .2em
    }


    .comments-block-outer > .comments-author {
        border: 0;
        border-bottom: 1px solid #cac4c4;
        padding-left: .2em;

    }

    .comments-text {
        white-space: pre-wrap;
        padding: .2em;
        font-size: 1em;
        background-color: #fff;
    }

    .button-bar {
        /*padding-top: 0.5em;
        padding-bottom: 0.5em;*/
    }

    .button-ok, .button-cancel, .button-reply {
        float: right;
        margin-right: 1em;
    }
</style>