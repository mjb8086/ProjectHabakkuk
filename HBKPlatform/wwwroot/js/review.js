console.log("Hello from review!");

var adjModel = Backbone.Model.extend({
    defaults: { name: "", rank: 0, mciIdx: 0 } 
});


var adjCollectionCtr = Backbone.Collection.extend({
    model: adjModel,
    url: "/api/adjectives/0",
    id: "mciIdx",
    parse: function (data) {
        return data.adjectives;
    }
});

var adjs = new adjCollectionCtr();

var renderListItem = function (itm) {
    if (!itm) { console.log("no itm"); return ""; }
    var html = '<li>' + itm['name'] + '</li>';
    return html;
};

var adjViewCtr = Backbone.View.extend({
    el: "#adjList",
    tagName: 'li',
    className: 'adj',

    initialize: function () {
        this.render();
    },
    template: renderListItem,
    render: function () {
        this.$el.html(this.template(this.model.attributes));
        return this;
    }

});

var adjList = new adjViewCtr({ 
    model: adjModel,
    collection: adjs,
    initialize: function () {
        console.log(this.collection); 
    }
});

adjs.fetch().then(function () { 
    console.log(JSON.stringify(adjs));
    console.log("itm 1" + JSON.stringify(adjs.findWhere({name:"Empathetic"})));
    adjList.render();
});


console.log("done");
