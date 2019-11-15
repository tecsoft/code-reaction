import Vue from 'vue'
import Vuex from 'vuex'
import * as CommitCriteria from '@/commits/store'

Vue.use(Vuex);

interface IToken {
    username: string,
    token: string,
    isAdmin ?: boolean,
}

// Requires securing
const account = {
    namespaced : true,
    state: {
        authenticated: null,
        username: "",
        token: "",
        isAdmin : null
    },

    mutations: {
        authenticate(state: any, info: IToken) {
            state.authenticated = true;
            state.username = info.username;
            state.token = info.token;
        },
        asAdmin(state: any, isAdmin: boolean) {
            state.isAdmin = isAdmin;
        }
    },
    getters: {
        username(state : any) {
            if (state.username === "") {
                return window.localStorage.getItem('crun');
            }
            return state.username;
        },
        token(state : any) {
            if (state.token === "") {
                return window.localStorage.getItem('crto');
            }
            return state.token;
        },
        isAuthenticated(state: any) {
            return state.authenticated;
        },
        isAdmin(state: any) {
            return state.isAdmin;
        }
    },
    actions: {
        loggedIn(context: any, info: IToken) {
            window.localStorage.setItem('crun', info.username);
            window.localStorage.setItem('crto', info.token);
            //window.localStorage.setItem('cria', info.isAdmin ? "true" : "false");  
            context.commit('authenticate', info);
        },
        asAdmin(context: any, isAdmin : boolean) {
            context.commit('asAdmin', isAdmin);
        }
    }
};


const referenceData = {
    namespaced: true,
    state: {
        projects : []
    },
    mutations: {
        setProjects(state: any, projects: Array<any>) {
            state.projects = projects;
        }
    },
    getters: {
        projects(state: any) {
            return state.projects;
        }
    },

    actions: {
        set(context: any, projects: Array<any>) {
            context.commit("setProjects", projects);
        }
    }
}

export default new Vuex.Store({
    modules: {
        account: account,
        commitCriteria: CommitCriteria.commitCriteria,
        referenceData: referenceData,
    }
});

