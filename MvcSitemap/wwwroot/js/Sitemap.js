
$(function () {

    $('#uploadForm').on('submit', function (e) {
        e.preventDefault();
        var formData = new FormData($(this)[0]);
        $.ajax({
            url: 'UploadFiles',
            type: 'POST',
            data: formData,
            async: false,
            success: function (data) {
            	console.log('ajax', data);
                //xmlCompare(data.xmlData, siteMapOriginal);
            },
            cache: false,
            contentType: false,
            processData: false
        })
    });

});
