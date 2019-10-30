<template>
    <div class="signin text-center">
        <b-form v-on:submit.prevent="">
            <h2>CODE REACTION</h2>
            <b-form-group>
                <b-form-input type="text"
                              v-model="username"
                              placeholder="Username (svn)"
                              required />
            </b-form-group>
            <b-form-group>
                <b-form-input type="text"
                              v-model="email"
                              placeholder="Email"
                              required />
            </b-form-group>
            <b-form-group>
                <b-form-input type="password"
                              v-model="password"
                              placeholder="Password"
                              required />
            </b-form-group>
            <b-form-group>
                <b-form-input type="password"
                              v-model="passwordConfirm"
                              placeholder="Confirm password"
                              required />
            </b-form-group>
            <b-button type="submit" variant="primary" v-on:click="doRegister">Sign Up !</b-button>

        </b-form>
        <b-alert variant="warning" :show="notSuccessful" >{{errorMessage}}</b-alert>
    </div>

</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import * as $ from 'jquery';

    @Component
    export default class RegisterPage extends Vue {
        private username: string = "";
        private password: string = "";
        private email: string = "";
        private passwordConfirm = "";
        public errorMessage: string = "";

        get notSuccessful(): boolean {
            return this.errorMessage.length > 0;
        }

        doRegister(): void {

            if (this.password !== this.passwordConfirm) {
                this.errorMessage = "Passwords don't match !";
            }
            else {
                this.errorMessage = "";

                $.ajax({
                    type: 'POST',
                    context: this,
                    url: '/api/users/register/',
                    data: { "newuser-username": this.username, "newuser-password": this.password, "newuser-email": this.email }
                })
                .done(function (this: RegisterPage, data: any) {
                    this.errorMessage = "";

                    $.ajax({
                        type: 'POST',
                        context: this,  // don't forget
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        url: "/token",
                        data: 'username=' + this.username + '&password=' + this.password + '&grant_type=password',
                    })
                    .done(function (this: RegisterPage, result: any) {
                        this.errorMessage = "";
                        this.$store.dispatch("account/loggedIn", { username: this.username, token: result.access_token });
                        this.$router.replace({ name: "commits" });

                    })
                    .fail(function (this: RegisterPage, xhr: JQueryXHR, status: any, error: string) {
                        this.errorMessage = xhr.responseText;
                    });

                })
                .fail(function (this: RegisterPage, xhr: JQueryXHR, status: any, error: string) {
                    this.errorMessage = error;
                });
            }
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