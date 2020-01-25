//window.onload = function AddButton() {
//    @for (int i = 0; i < 4; i++)
//    var codeblock[i] = `
//        {
//              <form>
//                <button id="Portfolio" asp-controller="PortFolio"
//                        asp-action="Portfolio"
//                        asp-route-id="@Model[i].Id">
//                    Go To Portfolio
//                </button>
//            </form>
//@if (Model[i].HasJoined)
//            {
//                <form>
//                    <button asp-controller="PortFolio"
//                            asp-action="Portfolio"
//                            asp-route-id="@Model[i].Id">
//                        Go To Portfolio
//                    </button>
//                </form>
//            }
//            else
//            {
//                <input id="Join" type="submit" value="Join" class="btn btn-primary" />
//            }
//        }`
//    var string = `ButtonDiv`
//    for (var i = 0; i < 4; i++) {
//        var element = document.getElementById(string + `${i}`);
//        element.innerHTML = codeblock;
//    }
//}
//# sourceMappingURL=file.js.map