<template>
    <tr v-if="isBreak" class="revision-line state-break">
        <td colspan="4" class="revision-line-text"><span>{{line.Text}} -- non-modified --</span></td>
    </tr>
    <tr v-else-if="!isBreak"
        v-bind:class="trClass"
        v-on:mouseover="onMouseOver"
        v-on:mouseleave="onMouseLeave">
        <td v-bind:class="tdNumberClass">{{line.RemovedLineNumber}}</td>
        <td v-bind:class="tdNumberClass">{{line.AddedLineNumber}}</td>
        <td class="revision-line-state">
            <line-actions v-if="showActions"
                               v-bind:line="line" 
                               v-bind:showActions="showActions" 
                               v-on:add-comment="openCommentDialog" 
                               v-on:like-line="likeLine" />
            {{symbol}}
        </td>
        <td class="revision-line-text">
            <span>{{line.Text}}</span>
            <code-line-like v-if="isLiked" 
                            v-on="$listeners"
                            v-bind:line="line"/>
            <comments-block v-for="comment in line.Comments"
                            v-bind:comment="comment"
                            v-on="$listeners"/>
            <new-comment-block v-bind:show="showEditor" 
                               v-on="$listeners"
                               v-on:posted-new-comment="postedNewComment" 
                               v-on:new-comment-cancelled="cancelledNewComment" />
        </td>
    </tr>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';
    import { Line, ChangeState } from '../types/types';
    import CodeLineLike from './code-line-like.vue';
    import LineActions from './line-actions.vue';
    import CommentsBlock from './comment-block.vue';
    import NewCommentBlock from './new-comment-block.vue';

    @Component({
        components: { CodeLineLike, LineActions, CommentsBlock, NewCommentBlock }
    })
    export default class CodeLine extends Vue {
        @Prop() line !: Line;

        showActions : boolean = false;
        showEditor : boolean = false;

        get symbol() : string {
            if (this.line.ChangeState === ChangeState.Added) return '+';
            if (this.line.ChangeState === ChangeState.Removed) return '-';
            return "";
        }

        get changeStateClass() {
            if (this.line.ChangeState === ChangeState.Added) return 'state-added';
            if (this.line.ChangeState === ChangeState.Removed) return 'state-removed';
            return "";
        }

        get tdNumberClass() {
            return 'revision-line-number ' + this.changeStateClass;
        }

        get tdLineClass(): string {
            if (this.line.ChangeState === ChangeState.Added) return 'revision-line-number state-added';
            if (this.line.ChangeState === ChangeState.Removed) return 'revision-line-number state-removed';
            return "revision-line-text";
        }

        get isBreak() : boolean {
            return this.line.ChangeState === ChangeState.Break;
        }

        get trClass() : string {
            return ("revision-line " + this.changeStateClass);
        }

        get isLiked(): boolean {
            return this.line.Likes.length > 0;
        }

        onMouseOver() : void {
            this.showActions = true;
        }

        onMouseLeave() : void {
            this.showActions = false;
        }

        openCommentDialog() : void {
            this.showEditor = true;
        }

        postedNewComment(event: any ) : void {
            this.$emit('new-code-comment', { Line: this.line, Text: event.message });
            this.showEditor = false;
        }

        cancelledNewComment() : void {
            this.showEditor = false;
        }

        likeLine(event:any) : void {
           this.$emit('like-line', { Line: this.line });
        }
    }
</script>

<style scoped>
    .revision-line:hover, .comments-line:hover {
        cursor: pointer;
        background-color: #efefef;
    }
    .revision-line-text, .revision-line-number, .revision-line-state {
        font-family: DroidSansMono, Monospace, Consolas, Courier;
        font-size: 0.85em;
    }

    .revision-line-state {
        width: 7%;
        border-right: 1px solid transparent;
    }

    .revision-line-text {
        white-space: pre-wrap;
    }

    .revision-line-number {
        display: table-cell;
        text-align: center;
        color: #808080;
        width: 48px;
        border-left: 1px solid #cac4c4;
        border-right: 1px solid #cac4c4;
    }

    .state-added {
        background-color: rgba(163, 255, 133, 0.33);
    }

    .state-removed {
        background-color: rgba(255, 194, 178, 0.33);
    }

    .state-break {
        text-align: center;
        background-color: #fefefe;
        border-top: 1px solid #cac4c4;
        border-bottom: 1px solid #cac4c4;
        color: #cac4c4;
        font-style: italic;
    }
    .state-break {
        text-align: center;
        background-color: #fefefe;
        border-top: 1px solid #cac4c4;
        border-bottom: 1px solid #cac4c4;
        color: #cac4c4;
        font-style: italic;
    }

    .state-break:hover {
        cursor: default;
        background-color: inherit;
    }
</style>