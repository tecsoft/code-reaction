<template>
    <span id="line-hover-panel" class="line-hover-in" >
        <b-button variant="primary" squared v-if="canLike" v-on:click="addLike"><i class="fa fa-heart"></i></b-button>
        <b-button variant="primary" squared v-if="canUnlike" ><i class="fa fa-heart"></i></b-button>
        <b-button variant="primary" squared v-on:click="addComment"><i class="fa fa-comment"></i></b-button>
    </span>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';

    @Component
    export default class LineActions extends Vue {
        @Prop() line !: any;

        get canLike(): boolean {
            return this.line.Author !== this.$store.getters["account/username"] && this.canUnlike === false;
        }

        get canUnlike(): boolean {
            var username = this.$store.getters["account/username"];
            for (let i = 0; i < this.line.Likes.length; i++) {
                if (this.line.Likes[i] === username ) {
                    return true;
                }
            }
            return false;
        }

        addLike(): void {
            this.$emit('like-line', { line: this.line });
        }

        addComment(): void {
            this.$emit('add-comment');
        }


    }
</script>

<style scoped>
    .line-hover-in, .line-hover-out {
        position: absolute;
        margin-left: 8px;
    }

    .line-hover-out {
        display: block;
    }

    .line-hover-in {
        display: block;
    }

    .line-hover-in > .btn {
       padding:0;
       padding-left:.2em;
       padding-right:.2em;
       vertical-align:middle;
    }
</style>