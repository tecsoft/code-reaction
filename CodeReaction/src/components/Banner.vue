<template>
    <div>
        <div class="title">CODE REACTION</div>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';

    @Component
    export default class Banner extends Vue {
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .title {
        font-size: 2.0em;
        color: #ffffff;
        text-align: center;
    }

    /*.project-selector {
        text-align: right;
        margin-right: 1em;
        float: right;
    }*/

    /*.header {
        background-color: #313131;
        color: #ffffff;
        min-height: 60px;
    }

    .header .title {
        font-size: 2.0em;
        color: #ffffff;
        text-align: center;
    }*/

    /*.header .project-selector {
        text-align: right;
        margin-right: 1em;
        float: right;
    }*/
</style>