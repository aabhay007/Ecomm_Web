var dataTable;
$(document).ready(function (){
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tblData').DataTable({

        //lengthMenu: [
        //    [5, 10, 15, 20, -1],
        //    [5, 10, 15, 20,"ALL"]
        //],
        "lengthMenu": [5, 10, 15, 20],

        "ajax": {
            "url":"/Admin/CoverType/GetAll"
        },
       
        "columns": [
         
            {

                "data": "id",
            
                "render": function (data) {
                    return `
                    <div>
                    <a href="/Admin/CoverType/Upsert/${data}" class="btn btn-info">
                    <i class="fas fa-edit"></i>
                    </a>
                    <a class="btn btn-danger" onclick=Delete('/Admin/CoverType/Delete/${data}')>
                    <i class="fas fa-trash-alt"></i>
                    </a>
                    </div>

                    `;

                }

            },   { "data": "name", "width": "50%" }
        ]
        })
}
function Delete(url) {
    swal({
        title: "Want to delete data?",
        text: "Delete Information",
        icon: "warning",
        buttons: true,
        dangerModel:true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
                })
        }
    })
}