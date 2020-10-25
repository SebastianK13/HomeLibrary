$(document).ready(function () {
    //$(".loader-wrapper").fadeOut(800);
    var noti = null;
    var onResizeTimer = null;
    var hoverActive = false;
    var eventOver = false;
    var menuActive = false;
    var lowerMenuActive = false;
    var lowerMenuMobileActive = false;
    var searchClone = $('#search-area').clone();
    var leftMenu = false;
    var lastLi = null;
    var liId = null;
    var name = null;
    var number = 0;
    var modelName = null;
    var value = null;
    var status = false;
    var setphoto = false;
    var setbckg = false;

    $('.menu-toggle').click(function () {
        if (lowerMenuMobileActive) {
            lowernavMenu();
        }
        $('nav').toggleClass('active');
        if (noti == null) {
            noti = $('#search-area').detach();
        }
        if ($('#searchUl').length == 0) {
            $('#navb').append($('<ul>').attr('class', 'searchLi').attr('id', 'searchUl'));
            $('#searchUl').append($('<li>').attr('class', 'new').append(searchClone));

            $('.search-bar').toggleClass('search-bar-activated').width("100%");
            $('.search-bar').removeClass('search-bar');
            $('#activate').removeClass();
            $('#activate').addClass('btn-search-activated');
            $('.search-text-box').toggleClass('search-text-box-activation');
            $('.search-text-box').removeClass('search-text-box');
        }

        if (!menuActive) {
            menuActive = true;
            $('.searchPlace').css('display', 'inline');
        }
        else if (menuActive) {
            menuActive = false;
        }

    })

    $("body").on("click", ".active .searchLi #close", function () {
        $('.active').removeClass('active');
        menuActive = false;
    });

    $('#search-area').mouseleave(function () {
        setTimeout(function () {
            var temp = document.getElementById('inpt').value;

            if (hoverActive == true && temp == "") {
                $('#search').toggleClass('search-bar-activated move').toggleClass('search-bar');
                $('#activate').toggleClass('btn-search-activated').toggleClass('btn-search');
                $('#inpt').toggleClass('search-text-box').toggleClass('search-text-box-activation');
                hoverActive = false;
            }
        }, 3000);
    });
    $('input[id=inpt]').on('keydown', function (e) {
        if (e.which == 13) {
            var temp = document.getElementById('inpt').value;
            if (temp != "") {
                window.location.href = '/Community/Users?searchValue=' + temp;
            }
        }
    });

    $(window).resize(function () {
        removeActive();
        addSearchbar();
        lowernavMenu();
    })

    $(window).bind('wheel mousewheel', function () {
        if (!eventOver && !menuActive) {
            $("html").css('overflow-y', 'auto')
            eventOver = true;
            setTimeout(function () {
                $("html").css('overflow-y', 'hidden')
                eventOver = false;
            }, 2500);
        }
    });

    $(window).on('mousewheel', function (e) {
        if (!eventOver && !menuActive) {
            $("html").css('overflow-y', 'auto')
            eventOver = true;
            setTimeout(function () {
                $("html").css('overflow-y', 'hidden')
                eventOver = false;
            }, 2500);
        }
    });

    $(window).on('touchmove', function () {
        if (!eventOver && !menuActive) {
            $("html").css('overflow-y', 'auto')
            eventOver = true;
            setTimeout(function () {
                $("html").css('overflow-y', 'hidden')
                eventOver = false;
            }, 2500);
        }
    });

    $(window).on('keydown', function (e) {
        if (e.which == 38 || e.which == 40) {
            if (!eventOver && !menuActive) {
                $("html").css('overflow-y', 'auto')
                eventOver = true;
                setTimeout(function () {
                    $("html").css('overflow-y', 'hidden')
                    eventOver = false;
                }, 2500);
            }
        }
    });

    $('.middle-part').click(function () {
        var screen = $(window);
        if (screen.width() < 991) {
            if (!lowerMenuMobileActive) {
                $('#account-menu').toggleClass('account-menu-section').toggleClass('account-menu-mobile');
                $('#md').toggleClass('middle-part').toggleClass('middle-part-unactive');
                lowerMenuMobileActive = true;
            }
            else if (lowerMenuMobileActive) {
                $('#account-menu').toggleClass('account-menu-mobile').toggleClass('account-menu-section');
                $('#md').toggleClass('middle-part-unactive').toggleClass('middle-part');
                lowerMenuMobileActive = false;
            }
        }
        else if (screen.width() > 991) {
            if (!lowerMenuActive) {
                lowerMenuActive = true;
                $('#bms').toggleClass('button-menu-section-cover').toggleClass('button-menu-section');
                $('#md').toggleClass('middle-part').toggleClass('middle-part-unactive');
            }
            else if (lowerMenuActive) {
                lowerMenuActive = false;
                $('#bms').toggleClass('button-menu-section').toggleClass('button-menu-section-cover');
                $('#md').toggleClass('middle-part-unactive').toggleClass('middle-part');
            }
        }
        $('#left-side').toggleClass('left-part').toggleClass('left-part-activated');
        $('#right-side').toggleClass('right-part').toggleClass('right-part-activated');
        $('#arrow-left').toggleClass('arrow-iddle').toggleClass('arrow-rotate');
        $('#arrow-right').toggleClass('arrow-iddle').toggleClass('arrow-rotate');
        if (menuActive == true) {
            removeActive();
        }
    })

    function removeActive() {
        $('nav').removeClass('active');
        menuActive = false;
    }

    function addSearchbar() {
        if (noti != null) {
            clearTimeout(onResizeTimer);
            onResizeTimer = setTimeout(function () {
                var screen = $(this);
                if (screen.width() > 991) {
                    noti.appendTo('#search-bar-li');
                    noti = null;
                    $('#searchUl').remove();
                }
            }, 10);
        }
        $('#new').remove();
    }

    function deactivate() {
        $('#bms').removeClass();
        $('#bms').addClass('button-menu-section-cover');
        $('#md').removeClass().addClass('middle-part');
        $('#left-side').toggleClass('left-part-activated').toggleClass('left-part');
        $('#right-side').toggleClass('right-part-activated').toggleClass('right-part');
        $('#arrow-left').toggleClass('arrow-iddle').toggleClass('arrow-rotate');
        $('#arrow-right').toggleClass('arrow-iddle').toggleClass('arrow-rotate');
    }

    function lowernavMenu() {
        var screen = $(window);
        if (screen.width() < 991) {
            if (lowerMenuActive) {
                deactivate();
                lowerMenuActive = false;
            }
            else if (lowerMenuMobileActive) {
                $('#account-menu').toggleClass('account-menu-mobile').toggleClass('account-menu-section');
                deactivate();
                lowerMenuMobileActive = false;
            }

        }
        else if (screen.width() > 991) {
            if (lowerMenuMobileActive) {
                $('#account-menu').toggleClass('account-menu-mobile').toggleClass('account-menu-section');
                $('#md').removeClass().addClass('middle-part');
                $('#left-side').toggleClass('left-part-activated').toggleClass('left-part');
                $('#right-side').toggleClass('right-part-activated').toggleClass('right-part');
                $('#arrow-left').toggleClass('arrow-iddle').toggleClass('arrow-rotate');
                $('#arrow-right').toggleClass('arrow-iddle').toggleClass('arrow-rotate');
                lowerMenuMobileActive = false;

            }
            if (leftMenu) {
                Restore();
            }
        }
    }
    $('.btn-search').hover(function f() {
        if (hoverActive == false) {
            hoverActive = true;
            $('#search').toggleClass('search-bar').toggleClass('search-bar-activated move');
            $('#inpt').toggleClass('search-text-box').toggleClass('search-text-box-activation');
            $('#activate').toggleClass('btn-search').toggleClass('btn-search-activated');
        }
    });

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#book-img').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $("#add-book-btn").change(function () {
        readURL(this);
    });
    $("#navbar-open").click(function () {
        $('#right-panel').toggleClass('right-panel-section').toggleClass('right-panel-section-on');
        $('#left-panel').toggleClass('left-panel-section').toggleClass('left-panel-section-off');
        $('#navbar-open').toggleClass('close-navbar-button').toggleClass('close-navbar-button-off');
        leftMenu = true;
    });
    $("#close-navbar").click(function () {
        Restore();
    });
    function Restore() {
        $('#right-panel').toggleClass('right-panel-section-on').toggleClass('right-panel-section');
        $('#left-panel').toggleClass('left-panel-section-off').toggleClass('left-panel-section');
        $('#navbar-open').toggleClass('close-navbar-button-off').toggleClass('close-navbar-button');
        leftMenu = false;
    }
    $('#modify').click(function () {
        SetDefault();
        Modify();
        window.location.href = '/Library/ManageBooks?actionNumber=' + '1';
    });
    function Modify() {
        $('.single-book-label img').toggleClass('label-icon').attr('src', '/Images/Modify_icon.png');
        $('.more-det').text('Modify');
        $('.single-book-label').toggleClass('single-book-label-modify');
        $('input[id = clicked-value]').attr('name', 'Modify,');
    }
    $('#remove').click(function () {
        SetDefault();
        Remove();
        window.location.href = '/Library/ManageBooks?actionNumber=' + '2';
    });
    function Remove() {
        $('.single-book-label img').toggleClass('label-icon').attr('src', '/Images/Remove_icon.png');
        $('.more-det').text('Remove');
        $('.single-book-label').toggleClass('single-book-label-remove');
        $('input[id = clicked-value]').attr('name', 'Remove,');
    }
    function SetDefault() {
        $('.single-book-label img').removeClass();
        $('.single-book-label').removeClass('single-book-label-modify').removeClass('single-book-label-remove');
    }
    //show book event
    $('.books div').click(function () {
        var option = $('#clicked-value').attr('name');
        var bookno = $(this).parent().parent().attr('name');
        var conc = option + bookno;
        $('input[id = clicked-value]').attr('name', conc);
        $('#book-form').submit();
    });
    $('.btn-borrowed').click(function () {
        var s = "borrowed";
        var bookno = $(this).attr('name');
        SetFormValue(s, bookno);
        $('#book-form').submit();
    });
    $('.btn-lend').click(function () {
        var s = "lend";
        var bookno = $(this).attr('name');
        SetFormValue(s, bookno);
        $('#book-form').submit();
    });
    $('.btn-available').click(function () {
        var s = "available";
        var bookno = $(this).attr('name');
        SetFormValue(s, bookno);
        $('#book-form').submit();
    });
    function SetFormValue(s, bookno) {
        var bookNumber = $(this).attr("name");
        var concat = bookNumber + " " + s;
        $('input[id = clicked-value-status]').attr('name', concat);
        $('input[id = clicked-value]').attr('name', bookno);

    }
    $('#modify-change').click(function () {
        window.location.href = '/Library/ManageBooks?actionNumber=' + '1';
    });
    $('#remove-change').click(function () {
        window.location.href = '/Library/ManageBooks?actionNumber=' + '2';
    });

    //InformationManager
    $('#Nickname-btn').click(function () {
        liId = "#Nickname-li";
        name = "Nickname";
        modelName = name;
        number = 2;
        value = document.getElementById('Nickname-btn').getAttribute('value');
        AddEditClass();
    });
    $('#Age-btn').click(function () {
        liId = "#Age-li";
        name = "Age";
        modelName = name;
        number = 2;
        value = document.getElementById('Age-btn').getAttribute('value');
        AddEditClass();
    });
    $('#Country-btn').click(function () {
        liId = "#Country-li";
        name = "Country";
        modelName = name;
        number = 3;
        value = document.getElementById('Country-btn').getAttribute('value');
        AddEditClass();
    });
    $('#City-btn').click(function () {
        liId = "#City-li";
        name = "City";
        modelName = name;
        number = 4;
        value = document.getElementById('City-btn').getAttribute('value');
        AddEditClass();
    });
    $('#Quote-btn').click(function () {
        liId = "#Quote-li";
        name = "Favourite quote";
        modelName = "Quote";
        number = 5;
        value = document.getElementById('Quote-btn').getAttribute('value');
        AddEditClass();
    });
    $('#Category-btn').click(function () {
        liId = "#Category-li";
        name = "Favourite book category";
        modelName = "FavouriteBookCategory";
        number = 6;
        value = document.getElementById('Category-btn').getAttribute('value');
        AddEditClass();
    });
    $('#Book-btn').click(function () {
        liId = "#Book-li";
        name = "Favourite book";
        modelName = "FavouriteBook";
        number = 7;
        value = document.getElementById('Book-btn').getAttribute('value');
        AddEditClass();
    });
    function AddEditClass() {
        var editField = document.createElement('li');
        $('#edit-li').remove();

        $(lastLi).show();
        $(liId).hide();
        $("#profile-info-list li:nth-child(" + number + ")").after(
            $("<li id=edit-li>" +
                "<div id=edit-field class=temp>" +
                "<div id=edit-field-name class=left-panel-left-label></div>" +
                "<div class=input-fields>" +
                "<div id=field-value class=field-value></div>" +
                "<div id=change-value class=left-panel-left-textbox>" +
                "<input id=input-edit class=form-control />" +
                "<input type=hidden id=edit-fieldsName name=fieldName>" +
                "</div>" +
                "</div>" +
                "<div id=profile-field-id class=left-panel-book-field>" +
                "<div class=buttons-edit-section>" +
                "<div id=save-changes class=button-edit>Save changes</div>" +
                "<div id=cancelAll class=button-cancel>Cancel</div>" +
                "</div>" +
                "</div>" +
                "</div>" +
                "</li >"
            ));
        $("#edit-field-name").text(name);
        SetValue();
        $("#edit-field-name").toggleClass('profile-field-name');
        $("#input-edit").toggleClass('medium-input').attr('name', modelName);
        $("#profile-field-id").toggleClass('profile-field-value-edit');

        lastLi = liId;

        if (liId === "#Book-li") {
            $('#edit-field').css("border-bottom", "none");
        }
    }
    function SetValue() {
        if (value != null && value != 0) {
            $("#field-value").text(value);
        }
        else {
            $("#field-value").text("unknown");
        }

    }
    $("#profile-info-list").on("click", "#cancelAll", function () {
        $(lastLi).show();
        $('#edit-li').remove();
    });
    $("#profile-info-list").on("click", "#save-changes", function () {
        $('input[name = fieldName]').attr('value', liId);
        $('#editProfile').submit();
    });

    $("#profile-info-list").on("mouseenter", "#field-value", function () {
        var c = document.getElementById('field-value');
        var text = c.textContent;
        if (text.length > 10) {
            $('#field-value').toggleClass('field-value-extended').removeClass('field-value');
            $('#input-edit').css('display', 'none');
        }
    });

    $("#profile-info-list").on("mouseleave", "#field-value", function () {
        var c = document.getElementById('field-value');
        var text = c.textContent;
        if (text.length > 10) {
            $('#field-value').removeClass('field-value-extended').toggleClass('field-value');
            $('#input-edit').css('display', 'block');
        }
    });
    //ManagePictures

    $("#remove-photo").click(function () {
        if (status == false) {
            $('.single-photo-img').toggleClass('single-photo-cover').removeClass('single-photo-img');
            status = true;
            $('.initial-btns').before(
                $("<div class=dynamically-btns>" +
                    "<label id=submit-changes class=submit-photo-btn>" +
                    "Apply" +
                    "</label>" +
                    "<label id=cancel-changes class=cancell-photo-btn>" +
                    "Cancell" +
                    "</label>" +
                    "</div>"
                ));
            Unchek();
        }
        else {
            $('.single-photo-cover').toggleClass('single-photo-img').removeClass('single-photo-cover');
            $('.dynamically-btns').remove();
            $('.single-photo-mask').remove();
            var elem = document.getElementsByClassName('single-photo-img');
            for (index = 0; index < elem.length; ++index) {
                elem[index].setAttribute('name', 'notclicked');
            }
            status = false;
        }
    });

    function Unchek() {
        if (setbckg == true || setphoto == true) {
            document.getElementById('set-bckg').checked = false;
            document.getElementById('set-photo').checked = false;
            setbckg = false;
            setphoto = false;
        }
    }

    $('.body-spec').on("click", ".photo-display-foreground", function () {
        $('.photo-display-foreground').remove();
    });

    $("#add-photo").change(function () {
        $('input[name = choseToRemove]').attr('value', '');
        $('#editProfile').submit();
    });

    $("#photos-id").on("mouseenter", ".single-photo-cover", function () {
        var t = $(this).attr('name');
        if (t == "notclicked") {
            var element = document.createElement("div");
            element.className = "single-photo-mask";
            var id = $(this).attr('id');
            element.setAttribute('id', id + 'r');

            document.getElementById(id).appendChild(element);
        }
    });

    $("#photos-id").on("mouseleave", ".single-photo-cover", function () {

        var p = $(this).children().attr('id');
        var t = $(this).attr('name');
        if (t == "notclicked") {
            $('#' + p).remove();
        }
    });

    $('.photos .single-photo').on("click", ".single-photo-img", function () {
        if (status == false) {
            var path = $(this).attr('value');
            $(".wrapper").after(
                "<div class=photo-display-foreground>" +
                "<div id=display class=photo-display>" +
                "<img id=bg-display class=background-display></img>" +
                "</div >" +
                "</div>");
            document.getElementById('bg-display').src = '../Images/' + path;
        }
    });

    $("#photos-id").on("click", ".single-photo-mask", function () {
        event.stopPropagation();
        var t = $(this).parent().attr('id');
        var n = $(this).parent().attr('name');
        var y = $('input[name = choseToRemove]').attr('value');
        if (n == "notclicked") {
            if (y == null) {
                y = t + ",";

            }
            else {
                y += t + ",";
            }
            $('#' + t).attr('name', 'clicked');
            $('input[name = choseToRemove]').attr('value', y);
        }
        else {
            var i = $(this).attr('id');
            $('#' + i).remove();
            $('#' + t).attr('name', 'notclicked');
            var g = y.replace(t + ',', '');
            $('input[name = choseToRemove]').attr('value', g);
        }
        if (setbckg == true) {
            $('input[name = choseBackground]').attr('value', t);
            $('#photosetterform').submit();
        }
        else if (setphoto == true) {
            $('input[name = choseProfilePic]').attr('value', t);
            $('#photosetterform').submit();
        }
    });
    $('#buttons-nav').on("click", "#submit-changes", function () {
        $('#editProfile').submit();
    });
    $('#buttons-nav').on("click", "#cancel-changes", function () {
        window.location.href = '/Community/PicturesProfile';
    });

    $('input[value=pictures]').prop('checked', false);

    $('#set-photo').click(function () {

    });
    $('#ph-chcbx').click(function () {
        if (setphoto == false) {
            CheckStatus();
            setphoto = true;
            setbckg = false;
            document.getElementById('set-photo').checked = true;
            document.getElementById('set-bckg').checked = false;
            $('.single-photo-img').toggleClass('single-photo-cover').removeClass('single-photo-img');
        }
        else {
            $('.single-photo-cover').toggleClass('single-photo-img').removeClass('single-photo-cover');
            CheckStatus();
            document.getElementById('set-photo').checked = false;
            setphoto = false;
        }
    });
    $('#set-bckg').click(function () {

    });

    $('#bckg-chcbx').click(function () {
        if (setbckg == false) {
            CheckStatus();
            setbckg = true;
            setphoto = false;
            document.getElementById('set-bckg').checked = true;
            document.getElementById('set-photo').checked = false;
            $('.single-photo-img').toggleClass('single-photo-cover').removeClass('single-photo-img');
        }
        else {
            $('.single-photo-cover').toggleClass('single-photo-img').removeClass('single-photo-cover');
            CheckStatus();
            document.getElementById('set-bckg').checked = false;
            setbckg = false;
        }
    });

    function CheckStatus() {
        $('.single-photo-cover').toggleClass('single-photo-img').removeClass('single-photo-cover');
        $('.dynamically-btns').remove();
        $('.single-photo-mask').remove();
        var elem = document.getElementsByClassName('single-photo-img');
        for (index = 0; index < elem.length; ++index) {
            elem[index].setAttribute('name', 'notclicked');
        }
        status = false;
    }

    //Profile View
    function AddProfileBtn() {
        var element = document.createElement("div");
        var p = document.createElement("p");
        var text = document.createTextNode("Add photo");
        p.appendChild(text);
        element.appendChild(p);
        element.className = "profile-photo-btn";
        element.setAttribute('id', 'add-profile-ph-btn');
        document.getElementById('profile-photo').appendChild(element);
        profile_active = true;
    };
    $('#profile-head').mouseenter(function () {
        var element = document.createElement("div");
        var text = document.createTextNode("+ Set background image");
        element.appendChild(text);
        element.className = "set-background-btn";
        element.setAttribute('id', 'background-btn');
        var id = $(this).attr('id');
        document.getElementById(id).appendChild(element);
        AddProfileBtn();
    });
    $('#profile-head').mouseleave(function () {
        $('#background-btn').remove();
        $('#add-profile-ph-btn').remove();
    });
    $('#profile-photo').on("click", "#add-profile-ph-btn", function () {
        event.stopPropagation();
        window.location.href = '/Community/PicturesProfile';
    });
    $('#profile-head').on("click", "#background-btn", function () {
        event.stopPropagation();
        window.location.href = '/Community/PicturesProfile';
    });
    $('#profile-head').on("click", "#profile-mask", function () {
        var path = $(this).parent().attr('value');
        ShowBiggerPhoto(path);
    });
    $('#profile-head').on("click", "#profile-photo", function () {
        var path = $(this).attr('value');
        ShowBiggerPhoto(path);
    });
    function ShowBiggerPhoto(path) {

        $(".wrapper").after(
            "<div class=photo-display-foreground>" +
            "<div id=display class=photo-display>" +
            "<img id=bg-display class=background-display></img>" +
            "</div >" +
            "</div>");
        document.getElementById('bg-display').src = '../Images/' + path;
    };
    //(partial) Search
    $('#search-bar-li').on("click", "#activate", function () {
        var temp = document.getElementById('inpt').value;
        if (temp != "") {
            window.location.href = '/Community/Users?searchValue=' + temp;
        }
    });
    //Users
    $('.add-friend-btn').on("click", function () {
        var id = $(this).parent().parent().attr('value');
        var parameter = id + ",add";
        $('input[name=pId]').attr('value', parameter);
        debugger;
        $('#userForm').submit();
    });
    $('.remove-friend-btn').on("click", function () {
        var id = $(this).parent().parent().attr('value');
        var parameter = id + ",remove";
        $('input[name=pId]').attr('value', parameter);
        $('#userForm').submit();
    });
    $('.invited-friend-btn').on("click", function () {
        var id = $(this).parent().parent().attr('value');
        var parameter = id + ",repeat";
        $('input[name=pId]').attr('value', parameter);
        $('#userForm').submit();
    });
    //Friends
    $('#invs-btn').on("click", function () {
        var n = $(this).attr('value');
        window.location.href = '/Community/Invitations?notiAmount=' + n;
    });
    $('#friends-btn').on("click", function () {
        window.location.href = '/Community/Friends';
    });
    $('.user').on("click", ".user-picture", ".user-info", function () {
        var id = $(this).parent().attr('value');
        $('input[name=fID]').attr('value', id);
        $('#fProfileForm').submit();
    });
    $('.user-info-name').on("click", function () {
        var id = $(this).parent().parent().parent().attr('value');
        $('input[name=fID]').attr('value', id);
        debugger;
        $('#fProfileForm').submit();
    });
    $('.user-info-loc').on("click", function () {
        var id = $(this).parent().parent().parent().attr('value');
        $('input[name=fID]').attr('value', id);
        debugger;
        $('#fProfileForm').submit();
    });
    //Invitation
    $('.accept-inv-btn').on("click", function () {
        var id = $(this).parent().parent().attr('value');
        var parameter = id + ",accept";
        $('input[name=invID]').attr('value', parameter);
        $('#invitationForm').submit();
    });
    $('#dec-inv').on("click", function () {
        var id = $(this).parent().parent().attr('value');
        var parameter = id + ",decline";
        $('input[name=invID]').attr('value', parameter);
        $('.decline-inv-btn').submit();
    });
    //FriendsUserList
    $('.invFriend').on("click", function () {
        var id = $(this).parent().parent().attr('value');
        var parameter = id + ",add";
        $('input[name=pId]').attr('value', parameter);
        debugger;
        $('#inviteForm').submit();
    });
    $('.removeFriend').on("click", function () {
        var id = $(this).parent().parent().attr('value');
        var parameter = id + ",remove";
        $('input[name=pId]').attr('value', parameter);
        $('#inviteForm').submit();
    });
    $('.repeatInv').on("click", function () {
        var id = $(this).parent().parent().attr('value');
        var parameter = id + ",repeat";
        $('input[name=pId]').attr('value', parameter);
        $('#inviteForm').submit();
    });
    //RateBook    
    $('.Rate-book-label div').mouseenter(function () {
        var nameClass = this.getAttribute("class");
        var starsArray = $(this).siblings('.RateBarBckg').children();
        var starsNum = $(this).parent('.Rate-book-label').attr("value");
        $('#starsNumber').attr("value", starsNum);
        switch (nameClass) {
            case "one-star":
                ReverseToGold(starsArray, 1);
                ReverseToBlank(starsArray, 1);
                break;
            case "two-stars":
                ReverseToGold(starsArray, 2);
                ReverseToBlank(starsArray, 2);
                break;
            case "three-stars":
                ReverseToGold(starsArray, 3);
                ReverseToBlank(starsArray, 3);
                break;
            case "four-stars":
                ReverseToGold(starsArray, 4);
                ReverseToBlank(starsArray, 4);
                break;
            case "five-stars":
                ReverseToGold(starsArray, 5);
                ReverseToBlank(starsArray, 5);
                break;
        }
    });

    $('.Rate-book-label div').mouseleave(function () {
        var amount = $('#starsNumber').attr("value");
        var starsArray = $(this).siblings('.RateBarBckg').children();
        ReverseToGold(starsArray, amount);
        ReverseToBlank(starsArray, amount);
    });

    function ReverseToGold(starsArray, amount) {
        for (i = 0; i < amount; i++) {
            starsArray[i].setAttribute("class", "gold-star");
        }
    };
    function ReverseToBlank(starsArray, amount) {
        for (i = amount; i < 5; i++) {
            starsArray[i].setAttribute("class", "blank-star");
        }
    };

    $('.Rate-book-label div').on("click", function () {
        var nameClass = this.getAttribute("class");
        var bookno = $(this).parents('.single-book').attr('name');
        $('input[id = book-id]').attr('value', bookno);

        debugger;

        switch (nameClass) {
            case "one-star":
                $('input[id = rate-field]').attr('value', 1);
                break;
            case "two-stars":
                $('input[id = rate-field]').attr('value', 2);
                break;
            case "three-stars":
                $('input[id = rate-field]').attr('value', 3);
                break;
            case "four-stars":
                $('input[id = rate-field]').attr('value', 4);
                break;
            case "five-stars":
                $('input[id = rate-field]').attr('value', 5);
                break;
        }
        $('#rate-form').submit();
    });
    $('.single-book-img').click(function () {
        var n = this;
        SubmitBookForm(n);
    });
    $('.book-informations').click(function () {
        var n = this;
        SubmitBookForm(n);
    });
    function SubmitBookForm(n) {
        var bookno = $(n).parents('.single-book').attr('name');
        var conc = bookno;
        $('input[id = clicked-value]').attr('name', conc);
        $('#book-form').submit();
    };
    //SwapBook
    $('#click').click(function () {
        $('#Swap-form').submit();
    });

    FillSwapBI = function CreateBookDynamic(obj) {
        var ID = $('#SwapID').val();

        var model = obj.filter(function (item) {
            return item.BookID == ID;
        });
        $('.single-book-img').removeClass('hideDiv');
        $('.book-informations').removeClass('hideDiv');

        var source = model[0].ImageSource;
        var id = model[0].BookID;
        var title = model[0].Title;

        $('#BookToSwap').attr("name", id);
        $('#SwapBookIMG').attr("src", '/Images/' + source);
        $('#SwapBookTitle').text(title);

        document.getElementById("offer").style.visibility = 'visible';
    };
    $("#SwapID").click(function () {
        $("option[value='-1']", this).hide();

    });
    $('#SwapAction').change(function () {

        var pickedVal = $(this).val();
        switch (pickedVal) {
            case "0":
                ClearAll();
                AddButtonOnly();
                ChangeListNameAttr("Away");
                break;
            case "1":
                ClearAll();
                AddDescField();
                AddButtonOnly();
                ChangeListNameAttr("SwapB");
                break;
            case "2":
                ClearAll();
                AddPriceField();
                AddButtonOnly();
                ChangeListNameAttr("Sell");
                break;
        }
    });
    $('#SwapAction').click(function () {
        $("option[value='-1']", this).hide();
    });
    function AddDescField() {
        var descDiv = document.createElement("div");
        descDiv.className = 'description-swap';
        descDiv.id = 'descSw';
        var descHead = document.createElement("label");
        descHead.className = 'desc-swap-head';
        descHead.textContent = 'Type which book you want exchange for';
        descDiv.appendChild(descHead);
        var descTextarea = document.createElement("textarea");
        descTextarea.name = 'DescSwap';
        descTextarea.id = 'SwapD';
        descTextarea.maxLength = '140';
        descTextarea.cols = '7';
        descTextarea.className = 'desc-text';
        descDiv.appendChild(descTextarea);
        var descBottom = document.createElement("label");
        descBottom.className = 'desc-swap-bottom';
        descBottom.textContent = 'Limit of characters: 150';
        descDiv.appendChild(descBottom);
        document.getElementById('offer').appendChild(descDiv);
    };
    function AddPriceField() {
        var inpt = document.createElement("input");
        inpt.id = 'money-field';
        inpt.placeholder = 'Set price';
        inpt.maxLength = '10';
        inpt.name = 'Price';
        var currencyList = document.createElement("select");
        currencyList.id = 'currency';
        currencyList.name = 'currency';
        var opt1 = document.createElement("option");
        opt1.value = 'PLN';
        opt1.text = 'PLN';
        var opt2 = document.createElement("option");
        opt2.value = 'EUR';
        opt2.text = 'EUR';
        var opt3 = document.createElement("option");
        opt3.value = 'USD';
        opt3.text = 'USD';
        var opt4 = document.createElement("option");
        opt4.value = 'CZK';
        opt4.text = 'CZK';
        var opt5 = document.createElement("option");
        opt5.value = 'default';
        opt5.text = 'Choose';
        currencyList.appendChild(opt1);
        currencyList.appendChild(opt2);
        currencyList.appendChild(opt3);
        currencyList.appendChild(opt4);
        currencyList.appendChild(opt5);

        document.getElementById('offer').appendChild(inpt);
        document.getElementById('offer').appendChild(currencyList);

    };
    function AddButtonOnly() {
        var submitBtn = document.createElement("div");
        submitBtn.id = 'confirm';
        submitBtn.className = 'btn-swap';
        submitBtn.textContent = 'Confirm';
        document.getElementById('offer').appendChild(submitBtn);
    }
    function ClearAll() {
        var descDiv = document.getElementById('descSw');
        var inpt = document.getElementById('currency');
        var currList = document.getElementById('money-field');
        if (descDiv != null) {
            var error = document.getElementById("descError");
            descDiv.parentNode.removeChild(descDiv);
            if (error != null) {
                error.parentNode.removeChild(error);
            }
        }
        else if (inpt != null || currList != null) {
            var error = document.getElementById("priceError");
            inpt.parentNode.removeChild(inpt);
            currList.parentNode.removeChild(currList);
            if (error != null) {
                error.parentNode.removeChild(error);
            }
        }
    };
    function ChangeListNameAttr(attrName) {
        var node = document.getElementById('SwapAction');
        var input = document.createElement("input");
        input.name = attrName;
        input.id = attrName;
        input.value = "true";
        node.appendChild(input);
    };
    $('#offer').on('click', '#confirm', function () {
        var pickedVal = $('#SwapAction').val();
        switch (pickedVal) {
            case "0":
                $('#Swap-form').submit();
                break;
            case "1":
                var node = document.getElementById('SwapD');
                var input = node.value;
                SwapValidation(input);
                break;
            case "2":
                var node = document.getElementById('money-field');
                var input = node.value;
                SetInputCurrencyValue();
                SellValidation(input);
                break;
        }
    });
    function SwapValidation(input) {
        if (input.length > 0) {
            $('#Swap-form').submit();
        }
        else {
            var condition = document.getElementById("descError");
            if (condition == null) {
                var node = document.getElementById('ErrorsSection');
                var ul = document.createElement("ul");
                ul.id = "descError";
                var li = document.createElement("li");
                li.textContent = "Description field cannot be empty";
                ul.appendChild(li);
                node.appendChild(ul);
            }
        }
    }
    function SellValidation(input) {
        if (input > 0) {
            $('#Swap-form').submit();
        }
        else {
            var condition = document.getElementById("priceError");
            if (condition == null) {
                var node = document.getElementById('ErrorsSection');
                var ul = document.createElement("ul");
                ul.id = "priceError";
                var li = document.createElement("li");
                li.textContent = "Value must be higher than 0";
                ul.appendChild(li);
                node.appendChild(ul);
            }
        }
    }
    function SetInputCurrencyValue() {
        var node = document.getElementById('SwapAction');
        var inputCurrency = document.createElement("input");
        inputCurrency.name = "Currency";
        inputCurrency.id = "Currency";
        var currencyValue = document.getElementById("currency");
        inputCurrency.value = currencyValue.value;
        node.appendChild(inputCurrency);
    }
    //TransactionOffers
    $('.ButtonOffer').click(function () {
        var actionName = this.getAttribute("value");
        var parameters = actionName.split(',');
        $('input[name = actionName]').attr('value', parameters[0]);
        $('input[name = SwapID]').attr('value', parameters[1]);
        $('#TransactionOffer-form').submit();
    });
    //AwayTransaction
    $('#confirm').click(function () {
        var swapID = $('#BookToSwap').attr("value");
        $('#SwapID').attr("value", swapID);

        var error = document.getElementById("descError");
        var desc = document.getElementById("SwapD").value.length;

        if (desc == 0) {
            var condition = document.getElementById("descError");
            if (condition == null) {
                var node = document.getElementById('ErrorsSection');
                var ul = document.createElement("ul");
                ul.id = "descError";
                var li = document.createElement("li");
                li.textContent = "Description offer cannot be empty";
                ul.appendChild(li);
                node.appendChild(ul);
            }
        }
        else {
            if (error != null) {
                error.parentNode.removeChild(error);
            }
            $('#AwayT-form').submit();
        }
    });
    //SellTransaction
    $('#acceptPrice').change(function () {
        var result = document.getElementById('acceptPrice').checked;
        if (result) {
            document.getElementById('money-field').disabled = true;
            document.getElementById('currency').disabled = true;
            var node = document.getElementById('offer');
            var mask = document.createElement('div');
            mask.textContent = "DISABLED";
            mask.className = "priceMask";
            mask.id = "priceMask";
            node.appendChild(mask);
            $('#accept').attr("value", "true");
        }
        else {
            var node = document.getElementById('priceMask');
            document.getElementById('money-field').disabled = false;
            document.getElementById('currency').disabled = false;
            node.parentNode.removeChild(node);
            $('#accept').attr("value", "false");
        }
    });
    $('#confirmSellT').click(function () {
        var swapID = $('#BookToSwap').attr("value");
        debugger;
        $('#SwapID').attr("value", swapID);
        $('#SellT-form').submit();
    });
    //SwapTransaction
    $("#ProposeID").click(function () {
        $("option[value='-1']", this).hide();

    });
    FillSwapTransactionBI = function CreateBookDynamic(obj) {
        var ID = $('#ProposeID').val();
        var error = document.getElementById("bookError");
        var model = obj.filter(function (item) {
            return item.BookID == ID;
        });
        if (model != null && error != null) {
            if (error != null) {
                error.parentNode.removeChild(error);
            }
        }
        $('.single-book-img').removeClass('hideDiv');
        $('.book-informations').removeClass('hideDiv');
        var source = model[0].ImageSource;
        var id = model[0].BookID;
        var title = model[0].Title;
        var swapId = $('#offerSwapT').attr("value");

        $('#SwapID').attr("value", swapId)
        $('#SwapBookIMGT').attr("src", '/Images/' + source);
        $('#SwapBookTitleT').text(title);
    };
    $('#Confirm').click(function () {
        var ID = $('#ProposeID').val();
        if (ID != -1) {
            $('#SwapT-form').submit();
        }
        else {
            var condition = document.getElementById("bookError");
            if (condition == null) {
                var node = document.getElementById('ErrorsSection');
                var ul = document.createElement("ul");
                ul.id = "bookError";
                var li = document.createElement("li");
                li.textContent = "Before confrim you need to choose book";
                ul.appendChild(li);
                node.appendChild(ul);
            }
        }
    });
    //Offers
    $('#transactions-btn').click(function () {
        window.location.href = '/Library/SwapTransactions';
    });
    $('#myOffers-btn').click(function () {
        window.location.href = '/Library/MyOffers';
    });
    $('#offers-btn').click(function () {
        window.location.href = '/Library/Offers';
    });
    $('#completed-btn').click(function () {
        window.location.href = '/Library/CompletedDeals';
    });
    //Offers in My transaction
    $('.offer-btn-acc').click(function () {
        var offerId = $(this).attr("value");
        $('#parameters').attr("value", offerId);
        $('#action').attr("value", "Accept");
        $('#Offers-form').submit();
    });
    $('.offer-btn-dec').click(function () {
        var offerId = $(this).attr("value");
        $('#parameters').attr("value", offerId);
        $('#action').attr("value", "Decline");
        $('#Offers-form').submit();
    });
    $('.offer-btn-commit').click(function () {
        var offerId = $(this).attr("value");
        $('#parameters').attr("value", offerId);
        $('#action').attr("value", "Commit");
        $('#Offers-form').submit();
    });
    $('.offer-btn-withdraw').click(function () {
        var offerId = $(this).attr("value");
        $('#parameters').attr("value", offerId);
        $('#action').attr("value", "Withdraw");
        $('#Offers-form').submit();
    });
    //MyOffers
    $('.myoffer-btn-commit').click(function () {
        var offerId = $(this).attr("value");
        $('#parameters').attr("value", offerId);
        $('#action').attr("value", "Commit");
        $('#MyOffers-form').submit();
    });
    $('.myoffer-btn-withdraw').click(function () {
        var offerId = $(this).attr("value");
        $('#parameters').attr("value", offerId);
        $('#action').attr("value", "Withdraw");
        $('#MyOffers-form').submit();
    });
    //_ChatPartial
    chatWindowBuilder = function buildChatWindow(parent) {
        var profileID = $(parent).attr('id');
        var singleWindowArea = document.createElement('div');
        singleWindowArea.className = "single-window-area";
        var chatWindow = document.createElement('div');
        chatWindow.className = "chat-window";
        chatWindow.id = profileID + "W";
        singleWindowArea.append(chatWindow);
        var chatWindowTop = document.createElement('div');
        chatWindowTop.className = "chat-window-top";
        chatWindowTop.textContent = $(parent).children('.friend-chat-name-container').text();
        chatWindowTop.id = profileID;
        $(chatWindow).append(chatWindowTop);
        var avatar = document.createElement('div');
        avatar.className = "friend-window-avatar";
        var avatarSource = $(parent).children('.friend-chat-avatar').css("background-image");
        avatar.style.backgroundImage = avatarSource;
        $(chatWindowTop).append(avatar);
        var closeBtn = document.createElement("div");
        closeBtn.className = "chat-close";
        $(chatWindowTop).append(closeBtn);
        var allmessages = document.createElement('div');
        allmessages.className = "all-messages";
        var messagesList = document.createElement('ul');
        messagesList.className = "messages-list";
        messagesList.id = profileID + "ML";
        $(allmessages).append(messagesList);
        $(chatWindow).append(allmessages);
        var chatWindowBottom = document.createElement('div');
        chatWindowBottom.className = "chat-window-bottom";
        var messageTB = document.createElement("textarea");
        messageTB.className = "message-text-box";
        messageTB.placeholder = "Type message...";
        messageTB.id = profileID + "TB";
        $(chatWindowBottom).append(messageTB);
        $(chatWindow).append(chatWindowBottom);
        $('#bottom-container-chat').append(singleWindowArea);
        return profileID;
    };
    incomingMessage = function buildChatOnReceiveMsg(message, senderInfo) {

        var parameters = senderInfo.split(',');
        var chatID = parameters[0] + 'W';
        var chatW = document.getElementById(chatID);
        var li = document.createElement('li');
        li.className = "senderLi";
        li.textContent = message;
        var messageList = $(chatW).children(".all-messages").children(".messages-list");
        var textbox = $(chatW).children(".all-messages").children(".messages-list").attr("id");
        $(messageList).append(li);
        var node = document.getElementById(textbox);
        node.scrollTop = node.scrollHeight;
        var html = $('#bottom-container-chat').html();
        window.sessionStorage.setItem('content', html);
    };
    addNewMessageToWindow = function newMsg(node, message) {
        var list = $(node).parent().siblings(".all-messages").children(".messages-list");
        var li = document.createElement('li');
        li.textContent = message;
        li.className = "receiverLi";
        $(list).append(li);
        var html = $('#bottom-container-chat').html();
        window.sessionStorage.setItem('content', html);
    };
    setCursor = function setCursorToStart(tbName) {
        var node = document.getElementById(tbName);
        node.setSelectionRange(-1, -1);
    };
    displayMessagesInWindow = function displayMessages(messages, friendID, myID) {
        var MessageList = document.getElementById(friendID + "ML");
        for (i = 0; i < messages.length; i++) {
            if (messages[i].OwnerID == myID) {
                var li = document.createElement('li');
                li.textContent = messages[i].Content;
                li.className = "receiverLi";
                $(MessageList).append(li);
            }
            else {
                var li = document.createElement('li');
                li.textContent = messages[i].Content;
                li.className = "senderLi";
                $(MessageList).append(li);
            }
        }
        var html = $('#bottom-container-chat').html();
        window.sessionStorage.setItem('content', html);
        var messageList = document.getElementById(friendID + "ML");
        messageList.scrollTop = messageList.scrollHeight;
    };

    CCW = function CreateChatWindow(senderInfo) {
        var parameters = senderInfo.split(',');
        var singleWindowArea = document.createElement('div');
        singleWindowArea.className = "single-window-area";
        var chatWindow = document.createElement('div');
        chatWindow.className = "chat-window";
        chatWindow.id = parameters[0] + "W";
        singleWindowArea.append(chatWindow);
        var chatWindowTop = document.createElement('div');
        chatWindowTop.className = "chat-window-top";
        chatWindowTop.textContent = parameters[1];
        chatWindowTop.id = parameters[0];
        $(chatWindow).append(chatWindowTop);
        var avatar = document.createElement('div');
        avatar.className = "friend-window-avatar";
        var avatarSource = parameters[2];
        //URL PATH CREATE
        avatar.setAttribute('style', 'background-image: url(/Images/' + avatarSource + ');');
        $(chatWindowTop).append(avatar);
        var closeBtn = document.createElement("div");
        closeBtn.className = "chat-close";
        $(chatWindowTop).append(closeBtn);
        var allmessages = document.createElement('div');
        allmessages.className = "all-messages";
        var messagesList = document.createElement('ul');
        messagesList.className = "messages-list";
        messagesList.id = parameters[0] + "ML";
        $(allmessages).append(messagesList);
        $(chatWindow).append(allmessages);
        var chatWindowBottom = document.createElement('div');
        chatWindowBottom.className = "chat-window-bottom";
        var messageTB = document.createElement("textarea");
        messageTB.className = "message-text-box";
        messageTB.placeholder = "Type message...";
        messageTB.id = parameters[0] + "TB";
        $(chatWindowBottom).append(messageTB);
        $(chatWindow).append(chatWindowBottom);
        $('#bottom-container-chat').append(singleWindowArea);
    }
    $('#bottom-container-chat').on("click", ".chat-close", function () {
        var node = $(this).parent().parent().parent();
        node.remove();
        var html = $('#bottom-container-chat').html();
        window.sessionStorage.setItem('content', html);
    });
    //Logout
    $("#logout").click(function () {
        sessionStorage.clear();
        $("#logoutForm").submit();
    });

});
