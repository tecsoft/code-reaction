/*
** File content component
*/

var fileBlock = {
    props: ['file'],
    name: 'file-block',
    components: { 'code-line': codeLine },

    methods: {
        expandFile: function () {
            var file = this.file;

            var mergeFunction = this.merge;

            var uri = '/api/commits/file/' + file.Revision + '?filename=' + encodeURIComponent(file.Name);

            $.getJSON(uri)
                .done(function (data, textStatus, jqXHR) {
                    // data contains lines of current revision so need to merge;
                    mergeFunction(file, data);
                });
        },

        merge: function (file, content) {

            var makeFileLine = function (lineNumber, fileName, revision, text) {
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

            var nbAll = content.length;
            var currentLineIndex = 0;
            var i;
            var item;
            var currentLines = file.Lines;

            while (currentLineIndex < currentLines.length) {

                if (currentLines[currentLineIndex].ChangeState === 3) {
                    // replace break with missing lines
                    var nextLineIndex = parseInt(currentLines[currentLineIndex].Id.split("_")[1]) - 1;
                    currentLines.splice(currentLineIndex, 1);

                    for (i = currentLineIndex; i < nextLineIndex; i++) {
                        currentLines.splice(i, 0, makeFileLine(i + 1, file.Name, file.Revision, content[i]));
                    }
                }
                else {
                    currentLineIndex++;
                }
            }
            // add any remaining lines to end
            for (i = currentLineIndex; i < nbAll; i++) {
                currentLines.splice(i, 0, makeFileLine(i + 1, file.Name, file.Revision, content[i]));
            }
        },

    },

    template:
        '<div>' +
        '<div class="file-header">' +
            '<button class="button-expand" v-on:click="expandFile">+</button>' +
            '<span class="label-change-type">{{file.ModText}}</span>' +
            '<span>{{file.Name}}</span>' +
        '</div>' +
        '<table class="revision-file-table">' +
            '<tbody>' +
                '<code-line v-for="line in file.Lines" v-bind:line="line"></code-line>' +
            '</tbody>' +
        '</table></div>'
};