<template>
    <div>
        <div class="file-header">
            <button class="btn btn-link" v-on:click="expandFile"><i class="fa fa-expand"></i></button>
            <span class="label-change-type">{{file.ModText}}</span>
            <span>{{file.Name}}</span>
        </div>

        <table class="revision-file-table">
            <tbody>
                <code-line v-for="line in file.Lines" v-bind:line="line" v-on="$listeners"></code-line>
            </tbody>
        </table>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import * as $ from 'jquery';
    import { File, ChangeState, Line } from '../types/types';
    import CodeLine from './code-line.vue';

    @Component({
        components: {CodeLine}
    })
    export default class FileBlock extends Vue {
        @Prop() file !: File;

        expandFile() {
            var file = this.file;
            var uri = '/api/review/file/' + file.Revision + '?filename=' + encodeURIComponent(file.Name);

            $.ajax({
                context: this,  // don't forget
                dataType: "json",
                url: uri,
                data: { revision: file.Revision, filename: file.Name },
                crossDomain: true
            })
                .done(function (this: FileBlock, data: any) {
                    
                this.merge(this.file, data);
            })
            .fail(function (xhr: JQueryXHR) {
                console.log(xhr.responseText);
            });
        }

        private merge(file: File, content: any): void {
            var makeFileLine = function (lineNumber : number, fileName : string, revision : number, text : string) : any {
                return {
                    AddedLineNumber: lineNumber,
                    RemovedLineNumber: lineNumber,
                    Id: lineNumber + "_" + lineNumber,
                    File: fileName,
                    Revision: revision,
                    Comments: [],
                    Text: text,
                    ChangeState: 0
                };
            };

            const nbAll : number = content.length;
            let currentLineIndex : number = 0;
            let currentLines : Line[] = file.Lines;

            while (currentLineIndex < currentLines.length) {

                if (currentLines[currentLineIndex].ChangeState === ChangeState.Break) {
                    // replace break with missing lines
                    var nextLineIndex = parseInt(currentLines[currentLineIndex].Id.split("_")[1]) - 1;
                    currentLines.splice(currentLineIndex, 1);

                    for (let i : number = currentLineIndex; i < nextLineIndex; i++) {
                        currentLines.splice(i, 0, makeFileLine(i + 1, file.Name, file.Revision, content[i]));
                    }
                }
                else {
                    currentLineIndex++;
                }
            }
            // add any remaining lines to end
            for (let i = currentLineIndex; i < nbAll; i++) {
                currentLines.splice(i, 0, makeFileLine(i + 1, file.Name, file.Revision, content[i]));
            }
        }
    }
</script>

<style scoped>
    .file-header {
        padding: 0.3em;
        background-color: #dfeef3;
        border-top-left-radius: 4px;
        border-top-right-radius: 4px;
        margin-top: 2em;
        border: 1px solid #cac4c4;
    }
    .revision-file-table {
        display: table;
        border-collapse: collapse;
        border-spacing: 0;
        vertical-align: top;
        width: 100%;
        border: 1px solid #cac4c4;
        margin-top: 0;
        padding: 0;
    }
    .label-change-type {
        margin-left: .2em;
        margin-right: .2em;
        float: right;
    }
</style>