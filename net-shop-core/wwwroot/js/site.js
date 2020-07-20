
// Write your JavaScript code.
$(document).ready(function () {

//    _____   _____    ____   _____   _    _   _____  _______     _____  __  __            _____  ______ 
//   |  __ \ |  __ \  / __ \ |  __ \ | |  | | / ____||__   __|   |_   _||  \/  |    /\    / ____||  ____|
//   | |__) || |__) || |  | || |  | || |  | || |        | |        | |  | \  / |   /  \  | |  __ | |__   
//   |  ___/ |  _  / | |  | || |  | || |  | || |        | |        | |  | |\/| |  / /\ \ | | |_ ||  __|  
//   | |     | | \ \ | |__| || |__| || |__| || |____    | |       _| |_ | |  | | / ____ \| |__| || |____ 
//   |_|     |_|  \_\ \____/ |_____/  \____/  \_____|   |_|      |_____||_|  |_|/_/    \_\\_____||______|
//                                                                                                       
//

    /**
    * Sets modal image for product
    *
    */
    $(".pr-main-img").click(function () {
        const img_link = $(this).attr('src');
        $("#modalImage").attr("src", img_link);
        $("#modalImageLink").attr("href", img_link);
        $('#imageModal').modal('show');
    });


    /**
    * Sets main product image to clicked image
    *
    */
    $(".pr-sub-img").click(function () {
        const host = window.location.origin;
        const img_link = host + "/" + $(this).attr('src');
        $(".pr-main-img").attr("src", img_link);
    });



//     _____  _    _   ____   _____   _____  _____  _   _   _____      _____            _____  _______ 
//    / ____|| |  | | / __ \ |  __ \ |  __ \|_   _|| \ | | / ____|    / ____|    /\    |  __ \|__   __|
//   | (___  | |__| || |  | || |__) || |__) | | |  |  \| || |  __    | |        /  \   | |__) |  | |   
//    \___ \ |  __  || |  | ||  ___/ |  ___/  | |  | . ` || | |_ |   | |       / /\ \  |  _  /   | |   
//    ____) || |  | || |__| || |     | |     _| |_ | |\  || |__| |   | |____  / ____ \ | | \ \   | |   
//   |_____/ |_|  |_| \____/ |_|     |_|    |_____||_| \_| \_____|    \_____|/_/    \_\|_|  \_\  |_|   
//                                                                                                     
//
    /**
    * Adds product data to shopping cart session data
    *
    * Get product data from add to cart button's data-*
    * 
    * Seperates different products by [#]
    * 
    */
    $(".cart-btn").click(function () {

        const product_id = $(this).data("product-id");
        const product_name = $(this).data("product-name");
        const product_price = $(this).data("product-price");
        const product_quantity = $(this).data("product-quantity");
        const product_image = $(this).data("product-image");
        const product_link = $(this).data("product-link");
        const session_data = product_id + "," + product_name + "," + product_price + "," + product_quantity + "," + product_image + "," + product_link + "[#]";
        const cart_data = $.session.get('ShoppingCart');
        $.session.set('ShoppingCart', cart_data + session_data);

        setCartNumber();//update total number in cart 

    });


    /**
    * Updates the total number of product items in  cart
    * 
    */
    setTimeout(
        function () {
            //wait 0.01 seconds and update total number in cart 
            setCartNumber();
        }, 10);


    /**
    * Updates the number of product quantity in data-quantity
    *
    * Disables cart button if product number is less than 0
    */
    $(".update-quantity").click(function () {
        setTimeout(
            function () {
                //wait 0.2 seconds for element to be updated
                const total_quantity = $("#total-quantity").val(); //get current quantity
                $('.cart-btn').data('product-quantity', total_quantity);

                //if quantity is 0, disable add to cart button
                if (parseInt(total_quantity) < 1) {
                    $(".cart-btn").addClass(" disabled");
                }
                else {
                    $(".cart-btn").removeClass("disabled");
                }

            }, 200);
    });


    /**
    * Clears shopping cart, then closes confirmation modal
    *
    */
    $("#ClearCart").click(function () {
        setTimeout(
            function () {
                $.session.set('ShoppingCart', "");
                $('#clearCartModal').modal('hide');
                location.reload();// reload page
            }, 100);
    });



    /**
    * Removes product from cart
    *
    */
    $('#ShoppingCartDiv').on('click', '#RemoveCartProduct', function () {
        setTimeout(
            function () {
                //get product data from remove button
                const product_id = $("a:focus").attr('data-product-id');
                const product_name = $("a:focus").attr("data-product-name");
                const product_price = $("a:focus").attr("data-product-price");
                const product_quantity = $("a:focus").attr("data-product-quantity");
                const product_image = $("a:focus").attr("data-product-image");
                const product_link = $("a:focus").attr("data-product-link");

                //create product data as in session data
                const product_data = product_id + "," + product_name + "," + product_price + "," + product_quantity + "," + product_image + "," + product_link + "[#]";
                var cart_data = formatCartSession();

                //if product data exist in session data, remove by replcing it with empty string
                if (cart_data.indexOf(product_data) >= 0) {
                    cart_data = cart_data.replace(product_data, '');
                }

                $.session.set('ShoppingCart', cart_data); //update cart data

                setCartNumber();//update total number in cart 

                location.reload();// reload page
            }, 100);
    });


    /**
    * Add one to total number in cart
    *
    */
    $('#ShoppingCartDiv').on('click', '.js-btn-plus', function (e) {
        e.preventDefault();
        $(this).closest('.input-group').find('.form-control').val(parseInt($(this).closest('.input-group').find('.form-control').val()) + 1);
    });

    /**
    * Subtract one from the total number in cart, if zero, set to 0
    *
    */
    $('#ShoppingCartDiv').on('click', '.js-btn-minus', function (e) {
        e.preventDefault();
        if ($(this).closest('.input-group').find('.form-control').val() != 0) {
            $(this).closest('.input-group').find('.form-control').val(parseInt($(this).closest('.input-group').find('.form-control').val()) - 1);
        } else {
            $(this).closest('.input-group').find('.form-control').val(parseInt(0));
        }
    });



    /**
    * Updates cart data to current data in cart
    *
    */
    $("#UpdateCart").click(function () {
        const host = window.location.origin;//get site host url
        var session_data = "";
        $('.table > tbody  > tr').each(function () { 
            $this = $(this);
            const product_id = $this.find("span.pr-id").html();
            const product_name = $this.find("h2.text-black").html();
            const product_price = $this.find("span.pr-price").html();
            const product_quantity = $this.find("input.pr-quantity").val();

            var product_image = $this.find("img.pr-image").attr("src");
            product_image = product_image.replace(host, "");
            if (product_image.indexOf("//") >= 0) {
                product_image = product_image.replace("//", "/");
            }

            var product_link = $this.find("a.pr-link").attr("href");
            product_link = product_link.replace(host, "");
            if (product_link.indexOf("//") >= 0) {
                product_link = product_link.replace("//", "/");
            }

            session_data += product_id + "," + product_name + "," + product_price + "," + product_quantity + "," + product_image + "," + product_link + "[#]";
        });

        $.session.set('ShoppingCart', session_data); //update cart data
        setCartNumber();//update total number in cart 
        location.reload();// reload page
    });


    /**
    * Checks if strings has undefined, replaces it with empty
    *
    * @return
    *   The value of the current session, null or string without 'undefined'.
    */
    function formatCartSession() {
        var cart_data = $.session.get('ShoppingCart');
        if (cart_data != "" && cart_data != null) {
            if (cart_data.length > 8 && cart_data.indexOf("undefined") >= 0) { 
                return cart_data.replace('undefined', '');
            }
            return cart_data;
        }

        return null;
    }

    /**
    * Updates the total number of item in cart
    *
    */
    function setCartNumber() {
        const total_num = getTotalInCart();
        $(".shopping-cart-count").text(total_num);
    }


    /**
    * Counts the number of item in cart
    *
    * @return
    *   The to total number of items in the shopping cart
    */
    function getTotalInCart() {
        var cart_data = $.session.get('ShoppingCart');
        var total_num = 0;
        if (cart_data != null) {
            $.each(cart_data.split("[#]").slice(0, -1), function (index, item) {
                total_num++;
            });
        }
        return total_num;
    }


    /**
    * Generates shopping cart table if in shopping cart checkout page
    *
    */
    setTimeout(
        function () {
            const host = window.location.origin;//get site host url
            const cart_data = formatCartSession(); //get current cart data
            $(".other-cart-components").show();//show Update, Shopping, and Coupon components

            //If in shopping cart page
            if ($('#ShoppingCartDiv').length && cart_data != null) {
                var content = "<table class='table table-bordered'>"
                content += `<thead>
	                        <tr>
		                        <th class='product-thumbnail'>Image</th>
		                        <th class='product-name'>Product</th>
		                        <th class='product-price'>Price</th>
		                        <th class='product-quantity'>Quantity</th>
		                        <th class='product-total'>Total</th>
		                        <th class='product-remove'>Remove</th>
	                        </tr>
                        </thead>`
                content += "<tbody>"
                //generate cart data here
                $.each(cart_data.split("[#]").slice(0, -1), function (index, item) {
                    var data_array = item.split(',');
                    content += `<tr> 
	                            <td class='product-thumbnail'>
                                    <a class='pr-link' href='`+ host + `/Shop/Details/` + data_array[5] + `'>
		                                <img class='pr-image' src='`+ host + `/` + data_array[4] + `' alt='Image' class='rounded' height='162' width='115'>
                                    </a>
	                            </td>
	                            <td class='product-name'>
		                            <h2 class='h5 text-black'>` + data_array[1] + `</h2>
                                    <span class='pr-id sr-only'>` + data_array[0] + `</span>
	                            </td>
	                            <td><span class='pr-price'>` + data_array[2] + `</span></td>
                                <td>
	                                <div class='input-group mb-3' style='max-width: 120px;'>
		                                <div class='input-group-prepend'>
			                                <button class='btn btn-outline-primary js-btn-minus' type='button'>&minus;</button>
		                                </div>
		                                <input type='text' class='form-control text-center pr-quantity' value='` + data_array[3] + `' placeholder='' aria-label='Example text with button addon' aria-describedby='button-addon1'>
		                                <div class='input-group-append'>
			                                <button class='btn btn-outline-primary js-btn-plus' type='button'>&plus;</button>
		                                </div>
	                                </div>

                                </td>
	                            <td>` + data_array[2] * data_array[3] + `</td>
	                            <td><a href='#' data-product-id='` + data_array[0] + `' data-product-name='` + data_array[1] + `' data-product-price='` + data_array[2] + `' data-product-quantity='` + data_array[3] + `' data-product-image='` + data_array[4] + `' data-product-link='` + data_array[5] + `' id='RemoveCartProduct' class='btn btn-primary btn-sm'>X</a></td>
                            </tr>`
                });
                content += "</tbody>"
                content += "</table>"

                $('#DynamicCartTable').append(content);
            }
            else {
                $('#EmptyCartText').text("Cart is empty");
                $(".other-cart-components").hide();//hide Update, Shopping, and Coupon components
            }
        }, 100);




//    ______  ____   _____   __  __    __      __       _       _____  _____         _______  _____  ____   _   _   _____ 
//   |  ____|/ __ \ |  __ \ |  \/  |   \ \    / //\    | |     |_   _||  __ \    /\ |__   __||_   _|/ __ \ | \ | | / ____|
//   | |__  | |  | || |__) || \  / |    \ \  / //  \   | |       | |  | |  | |  /  \   | |     | | | |  | ||  \| || (___  
//   |  __| | |  | ||  _  / | |\/| |     \ \/ // /\ \  | |       | |  | |  | | / /\ \  | |     | | | |  | || . ` | \___ \ 
//   | |    | |__| || | \ \ | |  | |      \  // ____ \ | |____  _| |_ | |__| |/ ____ \ | |    _| |_| |__| || |\  | ____) |
//   |_|     \____/ |_|  \_\|_|  |_|       \//_/    \_\|______||_____||_____//_/    \_\|_|   |_____|\____/ |_| \_||_____/ 
//                                                                                                                        
//                                                                                                                        

    /**
    * Checks if the Password and RepeatPassword fields match
    *
    */
    $("#RepeatPassword").keyup(function () {
        var password = $("#Password").val();
        var confirmPassword = $("#RepeatPassword").val();

        if (password == confirmPassword && password.length > 1) {
            $("#PasswordInfo").html("<strong class='text-success'>Passwords match.</strong>");
            $("#RegisterButton").attr("disabled", false);
        }
        else {
            $("#PasswordInfo").html("<strong class='text-danger'>Passwords do not match!</strong>");
            $("#RegisterButton").attr("disabled", true);
        }
    });


    /**
    * Disable buttons on click
    *
    */
    $("#SubmitButton, #RegisterButton").click(function () {
        //wait 0.1 seconds and disable submit button
        setTimeout(
            function () {
                $(this).prop('disabled', true);
            }, 100);
    });



    /**
    * Scroll to message div
    *
    */
    $('html, body').animate({
        scrollTop: ($("#ProcessMessages").offset().top - 50)
    }, 1000);

});