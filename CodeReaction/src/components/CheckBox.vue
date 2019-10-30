<template>
    <b-check v-bind:id="id"
             v-model="initial"
             v-on:change="changed" />
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator'
    @Component
    export default class CheckBox extends Vue {
        @Prop() id !: string;
        @Prop() checked !: boolean;

        private initial: boolean = this.checked;

        private changed(e: boolean): void {
            this.$emit("check-changed", { id: this.id, checked: e });
        }
    }
</script>

<style scoped>
</style>