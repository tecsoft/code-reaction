<template>
    <div class="comments-block">
        <div class="comments-author">
            <span v-if="line.Likes.length === 3"><strong>{{textOthers}}</strong> and <strong>1</strong> other liked this line</span>
            <span v-else-if="line.Likes.length > 3"><strong>{{textOthers}}</strong> and <strong>{{line.Likes.length-2}}</strong> others liked this line</span>
            <span v-else-if="line.Likes.length === 2"><strong>{{line.Likes[0]}}</strong> and <strong>{{line.Likes[1]}}</strong> liked this line</span>
            <span v-else-if="line.Likes.length === 1"><strong>{{line.Likes[0]}}</strong> liked this line</span>
            <span v-if="isLikedByUser">
                <button class="btn btn-link btn-xs" v-on:click="removeLike"><i class="fa fa-remove"></i></button>
            </span>
        </div>
        <div class="comments-text"></div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { Line } from '../types/types';

    @Component
    export default class CodeLineLike extends Vue {
        @Prop() line !: Line;

        get textOthers(): string {
            var username = this.$store.getters["account/username"];
            var reorderLikes = this.line.Likes.sort(
                function (a : string, b : string) {
                    return a === username ? -1 : 1;
                });

            return reorderLikes[0] + ", " + reorderLikes[1];
        }

        get isLikedByUser(): boolean {
            var username = this.$store.getters["account/username"];
            for (var i = 0; i < this.line.Likes.length; i++) {
                if (this.line.Likes[i] === username ) {
                    return true;
                }
            }

            return false;
        }

        removeLike(): void {
            this.$emit("unlike-line", this.line);
        }
    }
</script>

<style scoped>
</style>