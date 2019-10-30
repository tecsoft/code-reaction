<template>
    <div class="comment-nav">
        <b-button id="PreviousComment" variant="primary" v-on:click="goPreviousCommit">
            <i class="fa fa-chevron-up"></i>
        </b-button>
        <b-button id="NextComment" variant="primary" v-on:click="goNextCommit">
            <i class="fa fa-chevron-down"></i>
        </b-button>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { Commit } from '../types/types';

    @Component
    export default class CommentNav extends Vue {
        @Prop() commit !: Commit;
        indexPosComment: number = -1;

        goNextCommit(): void {

            let comments = document.getElementsByClassName('comments-block-outer');

            if (comments.length > 0) {
                this.indexPosComment++;

                if (this.indexPosComment >= comments.length) {
                    this.indexPosComment = 0;
                }

                let anchor = comments[this.indexPosComment];
                anchor.scrollIntoView();
            }
        }

        goPreviousCommit(): void {
            
            let comments = document.getElementsByClassName('comments-block-outer');

            if (comments.length > 0) {
                this.indexPosComment--;

                if (this.indexPosComment < 0) {
                    this.indexPosComment = comments.length-1;
                }

                let anchor = comments[this.indexPosComment];
                anchor.scrollIntoView();
            }
        }
    }
</script>

<style scoped>
    .comment-nav{
        position: fixed;
        top: 7px;
        right:18px;
    }
</style>