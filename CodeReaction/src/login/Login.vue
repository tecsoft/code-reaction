<template>
    <div class="signin text-center" v-cloak>
        <b-form v-on:submit.prevent="">
            <h2>CODE REACTION</h2>
            <b-form-group>
                <b-form-input type="text"
                              v-model="username"
                              placeholder="Username"
                              required />
            </b-form-group>
            <b-form-group>
                <b-form-input type="password"
                              v-model="password"
                              placeholder="Password"
                              required />
            </b-form-group>
            <b-button type="submit" variant="primary" v-on:click="doLogin">Log In</b-button>

        </b-form>
        <b-alert variant="warning" :show="notSuccessful">{{errorMessage}}</b-alert>
        <p>
            <router-link to="/register">Click here to register</router-link>
        </p>
    </div>
    
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import * as $ from 'jquery';

    @Component
    export default class LoginPage extends Vue {
        private username: string = "";
        private password: string = "";
        private errorMessage: string = "";

        get notSuccessful(): boolean {
            return this.errorMessage.length > 0;
        }

        doLogin(): void {
            $.ajax({
                type: 'POST',
                context: this,
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                url: "/token",
                data: 'username=' + this.username + '&password=' + this.password + '&grant_type=password',
                success: function (this: LoginPage, data: any) {
                    this.errorMessage = "";
                    this.$store.dispatch("account/loggedIn", { username: this.username, token: data.access_token });
                    let requestedPath = this.$router.currentRoute.query.returnUrl;
                    if (!requestedPath) {
                        requestedPath = "commits"
                    }
                    this.$router.replace({ path: requestedPath.toString() });
                },
                error: function (this: LoginPage, xhr: JQueryXHR) {
                    this.errorMessage = JSON.parse(xhr.responseText)["error_description"];
                }
            });
        }
    }
</script>

<style scoped>
    .signin {
        max-width: 330px;
        padding: 15px;
        margin: 0 auto;
        margin-top: 10%;
    }
</style>