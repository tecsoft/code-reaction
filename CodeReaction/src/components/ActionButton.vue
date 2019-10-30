<template>
    <b-button variant="outline" size="sm" v-on:click="clicked"><i v-bind:class="buttonText" /></b-button>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator'

    @Component
    export default class ActionButton extends Vue {
        @Prop() data !: any;
        @Prop() type !: string;

        get buttonText() : string {
            return "fa fa-" + this.type;
        }


        clicked(e: any): void {
            this.$emit("clicked", { data: this.data });
        }
    }
</script>

<style scoped>
</style>