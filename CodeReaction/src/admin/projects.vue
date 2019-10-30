<template>
    <div>
        <div v-if="isLoading">
            Loading ...
        </div>
        <!--<b-button variant="primary" squared v-on:click="addNew">Add new</b-button>-->
        <table v-if="projects.length > 0">
            <thead>
                <tr>
                    <th width="30%">Project</th>
                    <th>Path</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                <!--<tr v-if="isAdding" >
                    <td><input type="text" /></td>
                    <td><input type="text" /></td>
                    <td>
                        <action-button v-bind:data="'111'" v-on:clicked="doSave" v-bind:type="'save'" />
                        <action-button v-bind:data="'111'" v-on:clicked="" v-bind:type="'remove'" />
                    </td>
                </tr>-->
                <tr v-for="project in projects">
                    <td>{{project.Name}}</td>
                    <td>
                        {{project.Path}}
                    </td>
                    <td>
                        <!--<action-button v-if="!project.IsDefault" v-bind:data="project.Id" v-on:clicked="editProject" v-bind:type="'edit'" />-->
                    </td>
                </tr>
            </tbody>
        </table>
</div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import * as $ from 'jquery';
    import CheckBox from '@/components/CheckBox.vue';
    import ActionButton from '@/components/ActionButton.vue';

    interface IProject {
        Name: string;
        Id: number;
        Path: string;
        IsDefault: boolean;
    }

    @Component @Component({
        components: { CheckBox, ActionButton }
    })
    export default class ProjectsPage extends Vue {
        projects: Array<IProject> = [];
        isLoading: boolean = true;
        isAdding: boolean = false;

        private loadProjectList(): void {
            this.isLoading = true;
            $.ajax({
                context: this,
                dataType: "json",
                url: "api/projects",
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                .done(function (this: ProjectsPage, data: any) {
                    this.projects = data;
                })
                .fail(function (xhr: JQueryXHR) {
                    console.log(xhr.responseText);
                });

            this.isLoading = false
        }

        private addNew(): void {
            this.isAdding = true;
        }

        editProject(event: any) {
            alert(event.data);
        }

        created(): void {
            this.loadProjectList();
        }
    }
</script>

<style scoped>
</style>