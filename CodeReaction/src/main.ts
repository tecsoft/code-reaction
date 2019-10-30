import Vue from 'vue';
import App from './App.vue';

Vue.config.productionTip = false;

import VueRouter from 'vue-router';
import BootstrapVue from 'bootstrap-vue';
import Vuex from 'vuex';


import CommitPage from "@/commits/commit.vue";
import LoginPage from "@/login/Login.vue";
import RegisterPage from "@/login/Register.vue";
import ReviewPage from "@/review/Review.vue";
import FollowUpsPage from "@/followups/followups.vue";
import UsersPage from "@/admin/users.vue"
import ProjectsPage from "@/admin/projects.vue"

Vue.use(VueRouter);
const routes = [
    { path: '/', redirect: { name: "login" } },
    { path: '/login', name: "login", component: LoginPage },
    { path: '/register', name: "register", component: RegisterPage },
    { path: '/commits', name: "commits", component: CommitPage },
    { path: '/followups', name: "followups", component: FollowUpsPage },
    { path: '/review', name: "review", component: ReviewPage },
    { path: '/users', name: "users", component: UsersPage },
    { path: '/projects', name: "projects", component: ProjectsPage}
];
const router = new VueRouter({
    routes: routes,
    //mode : "history",
});

Vue.use(BootstrapVue);

import store from "@/store";

new Vue({
    store : store,
    router: router,
    render: h => h(App),
    
}).$mount('#app');

import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';