// Updates "Select all" control in a data table
function updateDataTableSelectAllCtrl(table) {
    var $table = table.table().node();
    var $chkbox_all = $('tbody input[type="checkbox"]', $table);
    var $chkbox_checked = $('tbody input[type="checkbox"]:checked', $table);
    var chkbox_select_all = $('thead input[name="select_all"]', $table).get(0);
    // If none of the checkboxes are checked
    if ($chkbox_checked.length === 0) {
        chkbox_select_all.checked = false;
        if ('indeterminate' in chkbox_select_all) {
            chkbox_select_all.indeterminate = false;
        }

        // If all of the checkboxes are checked
    } else if ($chkbox_checked.length === $chkbox_all.length) {
        chkbox_select_all.checked = true;
        if ('indeterminate' in chkbox_select_all) {
            chkbox_select_all.indeterminate = false;
        }

        // If some of the checkboxes are checked
    } else {
        chkbox_select_all.checked = true;
        if ('indeterminate' in chkbox_select_all) {
            chkbox_select_all.indeterminate = true;
        }
    }
}

function searchUrl(url) {
    url = url != "#search=all" ? url : " ";
    history.pushState(null, null, url);
}
$.fn.dataTable.ext.order['dom-checkbox'] = function (settings, col) {
    return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
        return $('input', td).prop('checked') ? '1' : '0';
    });
};
$(document).ready(function () {
    $('#BrandSelect').on('change', function () {
        window.location = "/sitemaps?Brand=" + $(this).val();
    })
    var cId = null;
    var deleteId = null;
    var formType = "";
    var container = "";
    var rootUrl = "/sitemaps/";
    // array holding selected row IDs
    var rows_selected = [];
    // get the query from the url
    var searchHash = location.hash.substr(1),
        searchString = searchHash.substr(searchHash.indexOf('search='))
            .split('&')[0]
            .split('=')[1]
    // create the table
    var table = $('#myTable').DataTable({
        searching: true,
        select: true,
        oSearch: { "sSearch": searchString != null ? decodeURI(searchString) : "" },
        style: 'multi',
        'columnDefs': [{
            'targets': 0,
            'searchable': false,
            'orderable': false,
            'width': '1%',
            'className': 'dt-body-center',
            'render': function (data, type, full, meta) {
                return '<input type="checkbox">';
            }
        },
        {
            'orderDataType': 'dom-checkbox',
            'targets': 1,
            'orderable': true
        }],
        'order': [2, 'desc'],
        'rowCallback': function (row, data, dataIndex) {
            // Get row ID
            var rowId = data[0];
            // If row ID is in the list of selected row IDs
            if ($.inArray(rowId, rows_selected) !== -1) {
                $(row).find('input[type="checkbox"]').prop('checked', true);
                $(row).addClass('selected');
            }
        },
        // search on keypress instead of keyup
        "initComplete": function (settings, json) {
            var textBox = $('#myTable_filter label input');
            textBox.unbind();
            textBox.bind('keyup input', function (e) {
                if (e.keyCode == 8 && !textBox.val() || e.keyCode == 46 && !textBox.val()) {
                    // do nothing 
                } else if (e.keyCode == 13 || !textBox.val()) {
                    table.search(this.value).draw();
                    var search = textBox.val();
                    if (search == null || search == "") {
                        search = 'all';
                    }
                    searchUrl('#search=' + search);
                }
            });
        }
    });// end datatable setup
    //OnClick of your button, redraw your Datatable
    $('#btnSearch').on('click', function () {
        table.fnDraw();
    });
    $('.update-table').hide();
    $('#myTable_length label').attr("style", "display: flex");
    $('#myTable_length select').attr("style", "margin: 0 5px");
    $('#myTable_filter label').attr("style", "display: flex;");
    $('#myTable_filter').attr("style", "padding: 0;").addClass("col-8");
    $('#matches-alert').hide();
    // Handle click on checkbox
    $('#myTable tbody').on('click', 'input[type="checkbox"]', function (e) {
        var $row = $(this).closest('tr');
        // Get row data
        var data = table.row($row).data();
        // Get row ID
        var rowId = data[0];
        // Determine whether row ID is in the list of selected row IDs 
        var index = $.inArray(rowId, rows_selected);
        // If checkbox is checked and row ID is not in list of selected row IDs
        if (this.checked && index === -1) {
            rows_selected.push(rowId);
            // Otherwise, if checkbox is not checked and row ID is in list of selected row IDs
        } else if (!this.checked && index !== -1) {
            rows_selected.splice(index, 1);
        }
        if (this.checked) {
            $row.addClass('selected');
        } else {
            $row.removeClass('selected');
        }
        // Update state of "Select all" control
        updateDataTableSelectAllCtrl(table);
        // Prevent click event from propagating to parent
        e.stopPropagation();
    });
    // Handle click on table cells with checkboxes
    $('#myTable').on('click', 'tbody td, thead th:first-child', function (e) {
        $(this).parent().find('input[type="checkbox"]').trigger('click');
    });
    // Handle click on "Select all" control
    $('thead input[name="select_all"]', table.table().container()).on('click', function (e) {
        if (this.checked) {
            $('#myTable tbody input[type="checkbox"]:not(:checked)').trigger('click');
        } else {
            $('#myTable tbody input[type="checkbox"]:checked').trigger('click');
        }
        // Prevent click event from propagating to parent
        e.stopPropagation();
    });
    // Handle form submission event 
    $('#form-sitemap').on('submit', function (e) {
        var form = this;
        // Iterate over all selected checkboxes
        $.each(rows_selected, function (index, rowId) {
            // Create a hidden element
            $(form).append(
                $('<input>')
                    .attr('type', 'hidden')
                    .attr('name', 'id[]')
                    .val(rowId)
            );
        });
        e.preventDefault();
    });

    $('#form-sitemap').on('click', '.delete-post', function (e) {
        e.preventDefault();
        $('#alert-deleted').hide();
        cId = $(this).attr('data-ID');
        modalCheck(rootUrl, 'Delete', cId);
        $("form#delete-form").click(function (e) {
            modalSubmit(rootUrl, "Delete", cId);
        });
        $('#modal-delete').on('hidden.bs.modal', function () {
            location.reload();
        });
    });

    $('#form-sitemap').on('click', '.edit-post', function (e) {
        e.preventDefault();
        $('#alert-editted').hide();
        cId = $(this).attr('data-ID');
        modalCheck(rootUrl, 'Edit', cId);
        $('form#edit-form').change(function () { // determine if there are changes in input fields
            modalSubmit(rootUrl, "EditPost", cId); // submit updated form
            $('#modal-edit').on('hidden.bs.modal', function () {
                location.reload();
            });
        });
    });


    $('#edit-bulk-post').on('click', function (e) {
        e.preventDefault();
        var rowData = table.rows('.selected').data().toArray();
        var idArray = rowData.map(function (element, i) {
            return parseInt(element.DT_RowId);
        });

        $('#alert-edit-bulk').hide();
        var url = rootUrl + 'EditBulkPost';
        $('form#edit-bulk-form').on('submit', function (e) {
            var pValue = $('form#edit-bulk-form').find('input[name=Priority]').val();
            var dataSet = {
                ID: idArray,
                Priority: pValue
            };
            e.preventDefault();
            $.ajax({
                url: url,
                type: 'POST',
                contentType: 'application/json',
                dataType: "json",
                data: JSON.stringify(dataSet),
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('An error has occured: ' + errorThrown);
                },
                success: function (data) {
                    $('#alert-edit-bulk').show();
                }
            });
            $('#modal-edit-bulk').on('hidden.bs.modal', function () {
                location.reload();
            });
        });
    });

    $('#delete-bulk-post').on('click', function (e) {
        e.preventDefault();
        $('#alert-delete-bulk').hide();
        var rowData = table.rows('.selected').data().toArray();
        var rowCount = table.rows('.selected').count();
        var idArray = rowData.map(function (element, i) {
            return parseInt(element.DT_RowId);
        });
        $('form#delete-bulk-form').on('submit', function (e) {
            e.preventDefault();
            var dataSet = {
                ID: idArray
            };
            $.ajax({
                url: rootUrl + 'DeleteBulk',
                type: 'POST',
                contentType: 'application/json',
                dataType: "json",
                data: JSON.stringify(dataSet),
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Unexpected Error Found: ", errorThrown);
                },
                success: function (data) {
                    $('#alert-delete-bulk').show().html(rowCount + " records have been deleted. You may close this pop up and review your changes.");
                }
            });
            $('#modal-delete-bulk').on('hidden.bs.modal', function () {
                location.reload();
            });
        });
    });

    $('#create-post').click(function (e) {
        e.preventDefault();
        $('#alert-saved').hide();
        modalSubmit(rootUrl, "CreatePost", cId); // submit updated form
        $('#modal-create').on('hidden.bs.modal', function () {
            location.reload();
        });
    });

    $('#uploadForm').on('submit', function (e) {
        e.preventDefault();
        var xmlFile = $('#fileChooser').prop('files')[0];
        var formData = new FormData();
        formData.append('file', xmlFile);
        $.ajax({
            url: 'sitemaps/UploadFiles',
            type: 'POST',
            data: formData,
            async: false,
            success: function (data) {
                if (data == "file not selected") {
                    $('#matches-alert').show().addClass("alert-warning").html("<strong>Please Select a File to Upload.</strong>");
                } else {
                    $('.update-table').show();
                    document.getElementById('indexPartial').innerHTML = data;
                    var newCheck = $('tbody .new').length;
                    var matchCheck = $('tbody .edit').length;
                    var deleteCheck = $('tbody .delete').length;
                    var matchCount = matchCheck + " Match(es) Found";
                    var newCount = newCheck + " New Item(s) were Added";
                    var deleteCount = deleteCheck + " Item(s) were Removed from the Table."
                    if (matchCheck == 0 && newCheck == 0 && deleteCheck == 0) {
                        $('#matches-alert').show().addClass("alert-danger").html("<strong>No Changes</strong> were made to the table. Please check your file or notify the Marketing Tech. team for further assisstance.");
                    } else {
                        $('#matches-alert').show().addClass("alert-success").html("<strong>Success!</strong> There are " + matchCount + ", " + newCount + ", and " + deleteCount);
                    }
                }
            },
            cache: false,
            contentType: false,
            processData: false
        });
    });

});

function modalCheck(root, action, id) {
    $.ajax({
        type: "GET",
        url: root + action + '/' + id,
        contentType: 'json',
        success: function (data) {
            var form;
            if (action == "Delete") {
                form = "#delete-form";
            } else {
                form = "#edit-form";
            }
            $('form' + form).find('input[name=NoIndex]').prop('checked', data.noIndex);
            $('form' + form).find('input[name=ID]').val(data.id);
            $('form' + form).find('input[name=Url]').val(data.url);
            $('form' + form).find('select[name=ChangeFrequency]').val(data.changeFrequency);
            $('form' + form).find('input[name=Priority]').val(data.priority);
        }
    });
}

function modalSubmit(root, action, id) {
    var action_btn = action;
    if (id != null) {
        action += "/" + id;
    }
    var form;
    if (action_btn == "Delete") {
        form = "#delete-form";
    } else if (action_btn == "EditPost") {
        form = "#edit-form";
    } else {
        form = "#create-form";
    }
    // Form Submit
    $('form' + form).submit(function (e) {
        e.preventDefault();
        postUrl = root + action;
        var dataToPost = $(this).serialize();
        var $btn = $(this).find('#btn-redeem');
        var $btnclose = $(this).find('#btn-close');
        $btn.button('loading');
        $.post(postUrl, dataToPost, function (data) {
            if (data) {
                if (action_btn == "EditPost" || action_btn == "CreatePost") {
                    $('#alert-saved').show().addClass("alert-success").html("This record has been saved. You may close this pop up.");
                    $('#alert-editted').show();
                    $btn.button('saved');
                    $btnclose.button('saved');
                }
                if (action_btn == "Delete") {
                    $('#alert-deleted').show();
                    $btn.button('deleted');
                    $btnclose.button('deleted');
                }
            }
        }).fail(function (xhr, status, error) {
            if (action_btn == "CreatePost") {
                $('#alert-saved').show().addClass("alert-danger").html("Failed! Please double check the values that were entered. \nNote: URL must be unique.");
            }
            $btn.button('reset');
        });
    });// end redeem modal
}