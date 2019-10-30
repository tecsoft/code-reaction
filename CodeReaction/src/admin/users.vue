<template>
    <div>
        <div v-if="isLoading">
            Loading ...
        </div>
        <table v-if="users.length > 0">
            <thead>
                <tr>
                    <th width="30%">User</th>
                    <th>Admin</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="user in users">
                    <td>{{user.UserName}}</td>
                    <td>
                        <check-box v-bind:id="user.UserName"  v-bind:checked="user.IsAdmin" v-on:check-changed="checkAdmin"/>
                    </td>
                    <td>
                        <action-button v-bind:data="user.UserName" v-on:clicked="resetUser" v-bind:type="'remove'"/>
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

    interface IUser {
        UserName: string;
        IsAdmin: boolean
    }

    @Component({
        components: { CheckBox, ActionButton }
    })
    export default class UsersPage extends Vue {
        private isLoading: boolean = true;
        private users: Array<IUser> = [];

        private loadUserList(): void {
            this.isLoading = true;
            $.ajax({
                context: this,
                dataType: "json",
                url: "api/users",
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                .done(function (this: UsersPage, data: any) {
                    this.users = data;
                })
                .fail(function (xhr: JQueryXHR) {
                    console.log(xhr.responseText);
                });

            this.isLoading = false
        }

        private checkAdmin(event : any) : void  {
            $.ajax({
                context: this,
                method: "POST",
                dataType: "json",
                url: "api/users/admin/" + event.id + "/" + event.checked,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                .done(function (this: UsersPage, data: any) {
            })
            .fail(function (xhr: JQueryXHR) {
                console.log(xhr.responseText);
            });
        }

        private resetUser(event: any): void {
            $.ajax({
                context: this,
                method: "POST",
                dataType: "json",
                url: "api/users/reset/" + event.data,
                beforeSend: function (xhr: JQueryXHR) {
                    xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                }
            })
                .done(function (this: UsersPage, data: any) {
                    this.users = this.users.filter(function (value: IUser, index: number, arr: Array<IUser>) {
                        return value.UserName !== event.data;
                    })
                })
                .fail(function (xhr: JQueryXHR) {
                    console.log(xhr.responseText);
                });
        }

        created(): void {
            this.loadUserList();
        }
    }
</script>

<style scoped>
    table {

        width: 100%;
    }
</style>