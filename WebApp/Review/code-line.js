/*
** Component code-line
*/
var codeLine = {
    props: ['line'],
    name: 'code-line',
    data: function () { return { showActions: false, showEditor: false }; },
    components: { 'code-line-actions': lineActions, 'comment-block': comment, 'new-comment-block': newComment, 'code-line-like' : codeLineLike },
    computed: {
        getSymbol: function () {
            if (this.line.ChangeState === 1) return '+';
            if (this.line.ChangeState === 2) return '-';
            return '';
        },

        getStateClass: function () {
            if (this.line.ChangeState === 1) return 'state-added';
            if (this.line.ChangeState === 2) return 'state-removed';
        },

        isBreak: function () {
            return this.line.ChangeState === 3;
        }
    },

    methods: {
        onMouseOver: function () {
            this.showActions = true;
        },

        onMouseLeave: function () {
            this.showActions = false;
        },

        openCommentDialog: function () {
            this.showEditor = true;
        },


        postedNewComment: function (message) {
            BUS.$emit('new-code-comment', { Line: this.line, Text: message });
            this.showEditor = false;
        },

        cancelledNewComment: function () {
            this.showEditor = false;
        },

        likeLine: function () {
            BUS.$emit('like-line', { Line: this.line });
        }
    },

    template:
        '<tr v-if="isBreak" class="revision-line state-break"><td colspan="4" class="revision-line-text">{{line.Text}} -- non-modified --</td></tr>' +
        '<tr v-else class="revision-line" v-bind:class="getStateClass"  v-on:mouseover="onMouseOver" v-on:mouseleave="onMouseLeave">' +
            '<td class="revision-line-number" v-bind:class="getStateClass">{{line.RemovedLineNumber}}</td>' +
            '<td class="revision-line-number" v-bind:class="getStateClass">{{line.AddedLineNumber}}</td>' +
            '<td class="revision-line-state">' +
                '<code-line-actions v-if="showActions" v-bind:line="line" v-bind:showActions="showActions" v-on:add-comment="openCommentDialog" v-on:like-line="likeLine" ></code-line-actions>' +
                '{{getSymbol}}' +
            '</td>' +
            '<td class="revision-line-text">{{line.Text}}' +
                '<code-line-like v-if="line.Likes.length > 0" v-bind:Likes="line.Likes"></code-line-like>' +
                '<comment-block  v-for="lineComment in line.Comments" v-bind:Comment="lineComment" ></comment-block>' +
                 '<new-comment-block v-bind:show="showEditor" v-on:posted-new-comment="postedNewComment" v-on:new-comment-cancelled="cancelledNewComment" />' +
            '</td>' +
         '</tr>'
};
