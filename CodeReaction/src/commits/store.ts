export interface ICommitSearchParameters {
    keyword: string,
    excludeMine: boolean,
    excludeApproved: boolean,
}

export const commitCriteria = {
    namespaced: true,
    state: {
        project: null,
        searchParameters: null
    },
    mutations: {
        setProject(state: any, id: number) {
            state.project = id;
        },
        setSearchParameters(state: any, searchParameters: ICommitSearchParameters) {
            state.searchParameters = searchParameters;
        }
    },

    getters: {
        project(state: any) {
            if (state.project == null) {
                return 1;
            }

            return state.project;
        },
        searchParameters(state: any) {
            return state.searchParameters;
        }
    },

    actions: {
        changeProject(context: any, id: number) {
            context.commit("setProject", id);
        },
        changeSearchParameters(context: any, searchParameters: ICommitSearchParameters) {
            context.commit("setSearchParameters", searchParameters);
        }
    }
}