
$(function () {
    $('#uploadForm').on('submit', function (e) {
        e.preventDefault();
        var xmlFile = $('#fileInput').prop('files')[0];
        var formData = new FormData();
        formData.append('file', xmlFile);
        $.ajax({
            url: 'UploadFiles',
            type: 'POST',
            data: formData,
            async: false,
            success: function (data) {
                console.log('ajax', data);
                //xmlCompare(data.xmlData, siteMapOriginal);
                document.getElementById('indexPartial').innerHTML = data;
            },
            cache: false,
            contentType: false,
            processData: false
        });
    });
});
