<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>lckey</title>
	<base target="_blank" />
	<link rel="shortcut icon" href="about:blank" />
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<meta name="description" content="幸运之钥,上网导航,网址大全,网址导航,个性导航,热点导航,便利服务" />
	<script src="a.js" type="text/javascript"></script>
	<script src="a.js" type="text/javascript"></script>
	<script src="b.js" type="text/javascript"></script>
	<script src="b.js" type="text/javascript"></script>
	<script src="b.js" type="text/javascript"></script>
	<script src="a.js" type="text/javascript"></script>
	<script src="a.js" type="text/javascript"></script>
	<script src="c.js" type="text/javascript"></script>
	<script src="js/loadScript.min.js" type="text/javascript"></script>
	<link href="css/index.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<div id="top-nav">
		<div class="box clearfix">
			<ul class="top-nav-left clearfix">
				<li>
					<span class="top-nav-tip">男孩穷着养 , 不然不晓得奋斗 ; 女孩富着养 , 不然人家一块蛋糕就哄走了..</span>
				</li>
				<li>
					<span>&nbsp;&nbsp;<a href="http://blog.renren.com/share/707577044/1000465222" title="更多笑话">&gt;&gt;</a></span>
					<!--<a href="http://blog.renren.com/share/707577044/1000465222" title="更多笑话">
						<span class="top-nav-tip-more">
							<span class="top-nav-tip-more"></span>
						</span>
					</a>-->
				</li>
			</ul>
			<ul class="top-nav-right">
				<li>
					<a href="javascript:void(0)" onclick="this.style.behavior='url(#default#homepage)';this.setHomePage('http://www.lckey.com/');return false;">设为首页</a>
				</li>
			</ul>
		</div>
	</div>
	<div id="head" class="box clearfix">
		<div class="head-logo">
			<!-- <span>www.</span>-->
			<span class="domain-name">LC</span>
			<span style="color: green;">key</span>
			<span>. com</span>
		</div>
		<div id="head-search">
			<input id="head-search-textbox" class="textbox-search" type="text" value="" />
			<input id="head-search-button" class="btn-submit" type="button" value="搜索" />
		</div>
	</div>
	<script type="text/javascript">
		//logo
		var logo = "www.lckey.com";
		//click style 按下样式
		var btnSearch = document.getElementById("head-search-button");
		var textbox = document.getElementById("head-search-textbox");
		var redirectByVal = function(val) {
			if (val) {
				var uri = "http://www.baidu.com/s?bs=s&f=8&rsv_bp=1&rsv_spt=3&wd=" + encodeURIComponent(val) + "&inputT=391";
				window.open(uri);
			}
		};
		btnSearch.onmousedown = function() {
			var that = this;
			this.style.backgroundPosition = "0 -138px";
			redirectByVal(textbox.value);
		};
		btnSearch.onmouseout = btnSearch.onmouseup = function() {
			this.style.backgroundPosition = "0 -104px";
		};
		//init suggest
		var initScripts = ["js/suggest.min.js"];
		t.use(initScripts, function() {
			var sug1 = new Suggest("head-search-textbox", {
				inputdelay: 100,
				onselected: function() {
					redirectByVal(textbox.value);
				}
			});
		});
		setTimeout(function() {
			//t.use(["js/log.min.js", "js/t.min.js"]);
		}, 8000);
	</script>
	<div id="article" class="box clearfix">
		<div id="left-article">
			<div id="left-article-hotspot">
				<h3>
					实时热点
				</h3>
				<ul>
					<li>
						<span><a href="http://v.youku.com/">视频</a>&nbsp;&nbsp;|&nbsp;&nbsp; </span>
						<a href="http://v.youku.com/v_show/id_XNDAwMDU4MTMy.html?f=17555926" class="">被朝扣押渔民:13天地狱经历 </a>
					</li>
					<li>
						<span><a href="http://news.ifeng.com/mainland/special/nanhaizhengduan/">专题</a>&nbsp;&nbsp;|&nbsp;&nbsp; </span>
						<a href="http://news.ifeng.com/mainland/special/nanhaizhengduan/content-3/detail_2012_05/21/14697672_0.shtml" class="">国防部回应中国战舰赴菲近海</a>
					</li>
					<li>
						<span><a href="http://ent.163.com/">娱乐</a>&nbsp;&nbsp;|&nbsp;&nbsp; </span>
						<a href="http://ent.163.com/12/0520/23/82034NI900031H2L.html" class="">陈冠希露骨短信挑逗女艺人</a>
					</li>
					<li>
						<span><a href="http://news.163.com/">新闻</a>&nbsp;&nbsp;|&nbsp;&nbsp; </span>
						<a href="http://news.163.com/12/0521/08/8212E05P00011229.html" class="">百亿富豪征妻 要求无性经验</a>
					</li>
				</ul>
			</div>
		</div>
		<div id="right-article">
			<div id="right-article-hot">
				<span><a href="http://www.baidu.com/">百 度</a> </span>
				<span><a href="http://www.sina.com.cn/">新 浪</a> </span>
				<span><a href="http://www.qq.com/">腾讯</a>• <a href="http://qzone.qq.com/">QQ空间</a> </span>
				<span><a href="http://www.sohu.com">搜 狐</a> </span>
				<span><a href="http://www.163.com">网 易</a> </span>
				<span><a href="http://www.google.com.hk/">谷 歌</a> </span>
				<span><a href="http://www.ifeng.com/">凤 凰 网</a> </span>
				<span><a href="http://weibo.com/">新浪微博</a> </span>
				<span><a href="http://www.youku.com/">优 酷 网</a> </span>
				<span><a href="http://www.xinhuanet.com">新 华 网</a> </span>
				<span><a href="http://www.10086.cn/service">移动营业厅</a> </span>
				<span><a href="http://www.cntv.cn/index.shtml">CNTV</a> </span>
				<span><a href="http://www.renren.com/">人 人 网</a> </span>
				<span><a href="http://www.kaixin001.com/">开 心 网</a> </span>
				<span><a href="http://www.4399.com/">4399游戏</a> </span>
				<span><a href="http://www.autohome.com.cn">汽车之家</a> </span>
				<span><a href="http://seer.61.com/?tmcid=85">赛 尔 号</a> </span>
				<span><a href="http://www.pconline.com.cn">太平洋电脑</a> </span>
				<div style="border-top: 1px dashed #CCE0C2; margin: 5px 15px 6px 15px;">
				</div>
				<span><a href="http://www.eastmoney.com">东方财富</a> </span>
				<span><a href="http://www.58.com">58 同 城</a> </span>
				<span><a href="http://www.zhcw.com">中 彩 网</a> </span>
				<span><a href="http://www.taobao.com">淘 宝 网</a> </span>
				<span><a href="http://www.meilishuo.com/">美 丽 说</a> </span>
				<span><a href="http://www.zhaopin.com/">智联招聘</a> </span>
				<span><a href="http://www.icbc.com.cn/">工商银行</a> </span>
				<span><a href="http://www.vancl.com/?source=hao123mph">凡客诚品</a> </span>
				<span><a href="http://www.zol.com.cn/">中关村在线 </a></span>
				<span><a href="http://www.gome.com.cn/?cmpid=dh_hao123_mz">国美电器</a> </span>
				<span><a href="http://www.bitauto.com/">易 车 网</a> </span>
				<span><a href="http://www.qunar.com/">去哪儿网</a> </span>
				<span><a href="http://click.union.360buy.com/JdClick/?unionId=75">京东商城</a> </span>
				<span><a href="http://www.suning.com/">苏宁易购</a> </span>
				<span><a href="http://www.nuomi.com/?cid=001601">糯 米 网</a> </span>
				<span><a href="http://www.amazon.cn/?tag=hao123com-famous-23">卓越亚马逊</a> </span>
				<span><a href="http://www.vipshop.com/g_url.php?g_id=12807">唯 品 会</a> </span>
				<span><a href="http://reg.jiayuan.com/st/?id=3237&amp;url=/st/main.php">世纪佳缘</a> </span>
			</div>
		</div>
	</div>
	<div id="foot" class="box">
		<p class="box-about span-rows">
			<a href="#">关于本站</a>
			<span>|</span>
			<a href="#">建议意见</a>
			<span>|</span>
			<a href="#">资助本站</a>
			<span>|</span>
			<a href="http://weibo.com/u/2105131203">官方微博</a>
			<span>|</span>
			<script type="text/javascript">
				var _bdhmProtocol = (("https:" == document.location.protocol) ? " https://" : " http://");
				document.write(unescape("%3Cscript src='" + _bdhmProtocol + "hm.baidu.com/h.js%3F39cb1a1ba2d3974d7d513deef034040d' type='text/javascript'%3E%3C/script%3E"));
			</script>
			<span>|</span>
			<script language="javascript" type="text/javascript" src="http://js.users.51.la/9345539.js"></script>
		</p>
		<p class="box-copyright span-rows">
			&copy; 2012 &nbsp; <a href="http://baike.baidu.com/view/1038598.htm" title="Luck">LC</a><span title="可以哦">KEY</span>.com , <a href="http://baike.baidu.com/view/353500.htm" title="Luck">幸运</a>之钥
		</p>
	</div>
</body>
</html>
