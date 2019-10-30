<template>
    <div id="app" class="wrapper" v-cloak>
        <div v-if="isAuthenticated" class="header">
            <Banner />
            <nav>
                <router-link v-if="isAuthenticated" to="/commits">Commits</router-link>
                <router-link v-if="isAuthenticated" to="/followups">Follow Ups</router-link>
                <router-link v-if="isAuthenticated && isAdmin" to="/users">Users</router-link>
                <!--<router-link v-if="isAuthenticated && isAdmin" to="/projects">Projects</router-link>-->
            </nav>
        </div>
        <div class="content">
            <router-view></router-view>
        </div>
        <div class="footer"/>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import * as $ from 'jquery';
    import Banner from './components/Banner.vue';
    @Component({
        components: {
            Banner
        }
    })
    export default class App extends Vue {

        get isAuthenticated() {
            var result = this.$store.getters["account/isAuthenticated"];
            if (result == null) {
                let username = this.$store.getters["account/username"];
                let token = this.$store.getters["account/token"];
                $.ajax({
                    context: this,
                    method: "POST",
                    url: "api/users/login/",
                    beforeSend: function (xhr: JQueryXHR) {
                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                    },
                    success: function (this: App, data: boolean) {
                        // refactor duplication of code in login.vue
                        this.$store.dispatch("account/loggedIn", { username: username, token: token });
                        let requestedPath = this.$router.currentRoute.query.returnUrl;
                        if (!requestedPath) {
                            requestedPath = "commits"
                        }
                        this.$router.replace({ path: requestedPath.toString() });
                    },
                    error: function () {
                        result = false;
                    }
                });

            }

            return result;
        }

        get isAdmin(): boolean {
            var result = this.$store.getters["account/isAdmin"];
            if (result == null) {
                $.ajax({
                    context: this,
                    method: "GET",
                    url: "api/users/admin/" + this.$store.getters["account/username"],
                    beforeSend: function (xhr: JQueryXHR) {
                        xhr.setRequestHeader("Authorization", "Bearer " + this.$store.getters["account/token"]);
                    },
                    success: function (this: App, data: boolean) {

                        this.$store.dispatch("account/asAdmin", data);
                    }
                    });
                
            }

            return result;
            
        }

        beforeCreate() {
            
            if (!this.isAuthenticated) {
                let currentRoute = this.$router.currentRoute;

                // TODO use router paths not strings
                if (currentRoute.fullPath == "/login" ){
                    this.$router.replace({ name: 'login' });
                }
                else if (currentRoute.fullPath.startsWith("/login?returnUrl") == false ){
                    this.$router.replace({ name: 'login', query: { returnUrl: currentRoute.fullPath } });
                }
            }
        };
    }
</script>

<style>

    @import 'assets/font-awesome.min.css';
    html, body {
        margin: 0;
        font-family: "Segoe UI", Helvetica Neue, Helvetica, Arial, sans-serif;
        height: 100%;
        font-size: 14px;
        line-height: 1.42857143;
        color: #333;
    }

    .header {
        background-color: #313131;
        color: #ffffff;
        min-height: 60px;
        flex-shrink: 0;
    }

    #container {
        position: relative;
        min-width: 92%;
        height: 88%;
    }

    .wrapper {
        width: 100%;
        height: 100%;
        display: flex;
        flex-direction: column;
        flex-wrap: nowrap;
    }

    .detailPanel2 {
        background-color: #ffffff;
        padding-left: 2%;
        padding-right: 2%;
        overflow-y: auto;
        overflow-x: auto;
        position: relative;
    }

    /*.header {
    }*/

    .footer {
        margin-top: .2em;
        margin-bottom: .2em;
    }

    .content {
        flex-grow: 1;
        overflow: auto;
        min-height: 2em;
        background-color: #ffffff;
        padding-left: 2%;
        padding-right: 2%;
        overflow-y: auto;
        overflow-x: auto;
        position: relative;
    }

    thead, tbody, tfoot {
        vertical-align: top;
    }

    td, th, tr {
        vertical-align: inherit;
        padding:0;
    }

    nav a {
        color: #fff;
        margin-right: 2px;
        padding: 10px 15px;
    }
    nav a:hover,
    nav a.router-link-active,
    nav a.router-link-exact-active {
        background-color: #fff;
        color: #555;
        cursor: pointer;
    }

    .badge .badge-primary {
        background-color: #337ab7;
    }

    .badge-warning > .badge {
        color: #fff;
        background-color: #f0ad4e;
    }

    [v-cloak] {
        display: none !important;
    }

    .btn {
        margin-left:0.5em;
    }
    
</style>
