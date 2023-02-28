$(document).ready(function() {
    //Helper function to keep table row from collapsing when being sorted
    var fixHelperModified = function(e, tr) {
        var $originals = tr.children();
        var $helper = tr.clone();
        $helper.children().each(function(index)
        {
            $(this).width($originals.eq(index).width())
        });
        return $helper;
    };
        
    //Make diagnosis table sortable
    $("#diagnosis_list tbody").sortable({
        helper: fixHelperModified,
        stop: function(event,ui) {renumber_table('#diagnosis_list')}
    }).disableSelection();

    //Save album order
    $("#albumSave").click(function () {
        var orderAlbum = [];
        $(".album").each(function () {
            orderAlbum.push($(this).attr('id'))
        });
        $.ajax({
            url: "AlbumSave",
            traditional: true,
            type: 'post',
            dataType: 'json',
            data: { 'order': orderAlbum },
            success: function (data) {
                console.log(data)
            }
        })
    })
    //photo save
    $("#photoSave").click(function () {
        var orderPhoto = [];
        $(".photo").each(function () {
            orderPhoto.push($(this).attr('id'))
        });
        $.ajax({
            url: "PhotoSave",
            traditional: true,
            type: 'post',
            dataType: 'json',
            data: { 'order': orderPhoto },
            success: function (data) {
                console.log(data)
            }
        })
    })
});
        
//Renumber table rows
function renumber_table(tableID) {
    $(tableID + " tr").each(function () {
        count = $(this).parent().children().index($(this)) + 1;
        $(this).find('.priority').html(count);
    });
}

