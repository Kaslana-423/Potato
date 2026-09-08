from __future__ import annotations

import datetime as dt
import io
import re
import tempfile
import urllib.parse
import zipfile
from pathlib import Path
from xml.etree import ElementTree as ET

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
README = ROOT / "README.md"
OUTPUT = ROOT / "Potato项目文档.docx"

NS = {
    "w": "http://schemas.openxmlformats.org/wordprocessingml/2006/main",
    "r": "http://schemas.openxmlformats.org/officeDocument/2006/relationships",
    "wp": "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing",
    "a": "http://schemas.openxmlformats.org/drawingml/2006/main",
    "pic": "http://schemas.openxmlformats.org/drawingml/2006/picture",
}
for prefix, uri in NS.items():
    ET.register_namespace(prefix, uri)


def q(prefix: str, name: str) -> str:
    return f"{{{NS[prefix]}}}{name}"


def element(parent: ET.Element, prefix: str, tag_name: str, **attrs: str) -> ET.Element:
    node = ET.SubElement(parent, q(prefix, tag_name))
    for key, value in attrs.items():
        if "__" in key:
            attr_prefix, attr_name = key.split("__", 1)
            node.set(q(attr_prefix, attr_name), str(value))
        else:
            node.set(key, str(value))
    return node


def xml_bytes(root: ET.Element) -> bytes:
    return ET.tostring(root, encoding="utf-8", xml_declaration=True)


class DocxBuilder:
    def __init__(self) -> None:
        self.document = ET.Element(q("w", "document"))
        self.body = element(self.document, "w", "body")
        self.relationships: list[tuple[str, str, str]] = [
            ("rId1", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles", "styles.xml"),
            ("rId2", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings", "settings.xml"),
            ("rId3", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header", "header1.xml"),
            ("rId4", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer", "footer1.xml"),
        ]
        self.media: list[tuple[str, bytes]] = []
        self.image_index = 0
        self.drawing_index = 0

    def add_paragraph(
        self,
        text: str = "",
        style: str | None = None,
        align: str | None = None,
        before: int | None = None,
        after: int | None = None,
        keep_next: bool = False,
        bold: bool = False,
        color: str | None = None,
        size: int | None = None,
        italic: bool = False,
    ) -> ET.Element:
        paragraph = element(self.body, "w", "p")
        props = element(paragraph, "w", "pPr")
        if style:
            element(props, "w", "pStyle", w__val=style)
        if align:
            element(props, "w", "jc", w__val=align)
        if before is not None or after is not None:
            spacing_attrs: dict[str, str] = {}
            if before is not None:
                spacing_attrs["w__before"] = str(before)
            if after is not None:
                spacing_attrs["w__after"] = str(after)
            element(props, "w", "spacing", **spacing_attrs)
        if keep_next:
            element(props, "w", "keepNext")
        if text:
            self.add_run(paragraph, text, bold=bold, color=color, size=size, italic=italic)
        return paragraph

    @staticmethod
    def add_run(
        paragraph: ET.Element,
        text: str,
        bold: bool = False,
        color: str | None = None,
        size: int | None = None,
        italic: bool = False,
        font: str | None = None,
    ) -> ET.Element:
        run = element(paragraph, "w", "r")
        props = element(run, "w", "rPr")
        if bold:
            element(props, "w", "b")
        if italic:
            element(props, "w", "i")
        if color:
            element(props, "w", "color", w__val=color)
        if size:
            element(props, "w", "sz", w__val=str(size))
            element(props, "w", "szCs", w__val=str(size))
        if font:
            element(props, "w", "rFonts", w__ascii=font, w__hAnsi=font, w__eastAsia=font)
        text_node = element(run, "w", "t")
        if text.startswith(" ") or text.endswith(" "):
            text_node.set("{http://www.w3.org/XML/1998/namespace}space", "preserve")
        text_node.text = text
        return run

    def add_page_break(self) -> None:
        paragraph = element(self.body, "w", "p")
        run = element(paragraph, "w", "r")
        element(run, "w", "br", w__type="page")

    def add_rule(self) -> None:
        paragraph = self.add_paragraph(after=180)
        props = paragraph.find(q("w", "pPr"))
        borders = element(props, "w", "pBdr")
        element(borders, "w", "bottom", w__val="single", w__sz="12", w__space="1", w__color="8D3B35")

    def add_quote(self, text: str) -> None:
        paragraph = self.add_paragraph(text, style="Quote")
        props = paragraph.find(q("w", "pPr"))
        borders = element(props, "w", "pBdr")
        element(borders, "w", "left", w__val="single", w__sz="20", w__space="8", w__color="A94D43")
        shading = element(props, "w", "shd", w__val="clear", w__color="auto", w__fill="F6E9D8")
        shading.set(q("w", "themeFillTint"), "00")

    def add_code(self, lines: list[str]) -> None:
        paragraph = self.add_paragraph(style="Code")
        props = paragraph.find(q("w", "pPr"))
        element(props, "w", "shd", w__val="clear", w__color="auto", w__fill="2A2020")
        for index, line in enumerate(lines):
            if index:
                run = element(paragraph, "w", "r")
                element(run, "w", "br")
            self.add_run(paragraph, line or " ", color="F5E9D3", size=18, font="Consolas")

    def add_table(self, rows: list[list[str]], widths: list[int] | None = None) -> None:
        if not rows:
            return
        column_count = max(len(row) for row in rows)
        widths = widths or [int(9000 / column_count)] * column_count
        table = element(self.body, "w", "tbl")
        props = element(table, "w", "tblPr")
        element(props, "w", "tblW", w__w="9000", w__type="dxa")
        borders = element(props, "w", "tblBorders")
        for edge in ("top", "left", "bottom", "right", "insideH", "insideV"):
            element(borders, "w", edge, w__val="single", w__sz="5", w__space="0", w__color="C9AA8D")
        element(props, "w", "tblCellMar", w__top="100", w__left="120", w__bottom="100", w__right="120")
        grid = element(table, "w", "tblGrid")
        for width in widths:
            element(grid, "w", "gridCol", w__w=str(width))

        for row_index, values in enumerate(rows):
            row = element(table, "w", "tr")
            if row_index == 0:
                row_props = element(row, "w", "trPr")
                element(row_props, "w", "tblHeader")
            for column_index in range(column_count):
                value = values[column_index] if column_index < len(values) else ""
                cell = element(row, "w", "tc")
                cell_props = element(cell, "w", "tcPr")
                element(cell_props, "w", "tcW", w__w=str(widths[column_index]), w__type="dxa")
                if row_index == 0:
                    element(cell_props, "w", "shd", w__val="clear", w__color="auto", w__fill="7B2F2B")
                elif row_index % 2 == 0:
                    element(cell_props, "w", "shd", w__val="clear", w__color="auto", w__fill="FBF5EB")
                paragraph = element(cell, "w", "p")
                paragraph_props = element(paragraph, "w", "pPr")
                element(paragraph_props, "w", "spacing", w__after="0", w__line="276", w__lineRule="auto")
                self.add_run(
                    paragraph,
                    clean_inline(value),
                    bold=row_index == 0,
                    color="FFFFFF" if row_index == 0 else "3D2A26",
                    size=19,
                )
        self.add_paragraph(after=80)

    def add_image(self, path: Path, width_inches: float = 6.25, caption: str | None = None) -> None:
        if not path.exists():
            self.add_quote(f"图片缺失：{path.relative_to(ROOT)}")
            return
        data = path.read_bytes()
        with Image.open(io.BytesIO(data)) as image:
            pixel_width, pixel_height = image.size
        aspect = pixel_height / max(pixel_width, 1)
        width_inches = min(width_inches, 6.35)
        height_inches = width_inches * aspect
        if height_inches > 7.8:
            height_inches = 7.8
            width_inches = height_inches / max(aspect, 0.01)

        self.image_index += 1
        self.drawing_index += 1
        extension = path.suffix.lower().lstrip(".") or "png"
        if extension == "jpg":
            extension = "jpeg"
        media_name = f"image{self.image_index}.{extension}"
        relation_id = f"rId{10 + self.image_index}"
        self.relationships.append(
            (
                relation_id,
                "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image",
                f"media/{media_name}",
            )
        )
        self.media.append((media_name, data))

        cx = int(width_inches * 914400)
        cy = int(height_inches * 914400)
        paragraph = self.add_paragraph(align="center", after=80)
        run = element(paragraph, "w", "r")
        drawing = element(run, "w", "drawing")
        inline = element(drawing, "wp", "inline", distT="0", distB="0", distL="0", distR="0")
        element(inline, "wp", "extent", cx=str(cx), cy=str(cy))
        element(inline, "wp", "effectExtent", l="0", t="0", r="0", b="0")
        element(inline, "wp", "docPr", id=str(self.drawing_index), name=f"Picture {self.drawing_index}")
        frame_props = element(inline, "wp", "cNvGraphicFramePr")
        element(frame_props, "a", "graphicFrameLocks", noChangeAspect="1")
        graphic = element(inline, "a", "graphic")
        graphic_data = element(graphic, "a", "graphicData", uri="http://schemas.openxmlformats.org/drawingml/2006/picture")
        picture = element(graphic_data, "pic", "pic")
        non_visual = element(picture, "pic", "nvPicPr")
        element(non_visual, "pic", "cNvPr", id="0", name=path.name)
        element(non_visual, "pic", "cNvPicPr")
        fill = element(picture, "pic", "blipFill")
        element(fill, "a", "blip", r__embed=relation_id)
        stretch = element(fill, "a", "stretch")
        element(stretch, "a", "fillRect")
        shape = element(picture, "pic", "spPr")
        transform = element(shape, "a", "xfrm")
        element(transform, "a", "off", x="0", y="0")
        element(transform, "a", "ext", cx=str(cx), cy=str(cy))
        geometry = element(shape, "a", "prstGeom", prst="rect")
        element(geometry, "a", "avLst")
        if caption:
            self.add_paragraph(caption, style="Caption", align="center", after=220)

    def finish(self) -> None:
        section = element(self.body, "w", "sectPr")
        element(section, "w", "headerReference", w__type="default", r__id="rId3")
        element(section, "w", "footerReference", w__type="default", r__id="rId4")
        element(section, "w", "pgSz", w__w="11906", w__h="16838")
        element(section, "w", "pgMar", w__top="1134", w__right="1134", w__bottom="1134", w__left="1134", w__header="567", w__footer="567", w__gutter="0")

    def write(self, output: Path) -> None:
        self.finish()
        with zipfile.ZipFile(output, "w", compression=zipfile.ZIP_DEFLATED) as archive:
            archive.writestr("[Content_Types].xml", content_types_xml())
            archive.writestr("_rels/.rels", package_relationships_xml())
            archive.writestr("docProps/core.xml", core_properties_xml())
            archive.writestr("docProps/app.xml", app_properties_xml())
            archive.writestr("word/document.xml", xml_bytes(self.document))
            archive.writestr("word/styles.xml", styles_xml())
            archive.writestr("word/settings.xml", settings_xml())
            archive.writestr("word/header1.xml", header_xml())
            archive.writestr("word/footer1.xml", footer_xml())
            archive.writestr("word/_rels/document.xml.rels", document_relationships_xml(self.relationships))
            for name, data in self.media:
                archive.writestr(f"word/media/{name}", data)


def content_types_xml() -> bytes:
    ns = "http://schemas.openxmlformats.org/package/2006/content-types"
    root = ET.Element(f"{{{ns}}}Types")
    for extension, content_type in (
        ("rels", "application/vnd.openxmlformats-package.relationships+xml"),
        ("xml", "application/xml"),
        ("png", "image/png"),
        ("jpeg", "image/jpeg"),
    ):
        ET.SubElement(root, f"{{{ns}}}Default", Extension=extension, ContentType=content_type)
    overrides = (
        ("/word/document.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"),
        ("/word/styles.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml"),
        ("/word/settings.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml"),
        ("/word/header1.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml"),
        ("/word/footer1.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml"),
        ("/docProps/core.xml", "application/vnd.openxmlformats-package.core-properties+xml"),
        ("/docProps/app.xml", "application/vnd.openxmlformats-officedocument.extended-properties+xml"),
    )
    for part, content_type in overrides:
        ET.SubElement(root, f"{{{ns}}}Override", PartName=part, ContentType=content_type)
    return ET.tostring(root, encoding="utf-8", xml_declaration=True)


def package_relationships_xml() -> bytes:
    ns = "http://schemas.openxmlformats.org/package/2006/relationships"
    root = ET.Element(f"{{{ns}}}Relationships")
    relations = (
        ("rId1", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "word/document.xml"),
        ("rId2", "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties", "docProps/core.xml"),
        ("rId3", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties", "docProps/app.xml"),
    )
    for relation_id, relation_type, target in relations:
        ET.SubElement(root, f"{{{ns}}}Relationship", Id=relation_id, Type=relation_type, Target=target)
    return ET.tostring(root, encoding="utf-8", xml_declaration=True)


def document_relationships_xml(relations: list[tuple[str, str, str]]) -> bytes:
    ns = "http://schemas.openxmlformats.org/package/2006/relationships"
    root = ET.Element(f"{{{ns}}}Relationships")
    for relation_id, relation_type, target in relations:
        ET.SubElement(root, f"{{{ns}}}Relationship", Id=relation_id, Type=relation_type, Target=target)
    return ET.tostring(root, encoding="utf-8", xml_declaration=True)


def core_properties_xml() -> bytes:
    cp = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties"
    dc = "http://purl.org/dc/elements/1.1/"
    dcterms = "http://purl.org/dc/terms/"
    xsi = "http://www.w3.org/2001/XMLSchema-instance"
    ET.register_namespace("cp", cp)
    ET.register_namespace("dc", dc)
    ET.register_namespace("dcterms", dcterms)
    ET.register_namespace("xsi", xsi)
    root = ET.Element(f"{{{cp}}}coreProperties")
    ET.SubElement(root, f"{{{dc}}}title").text = "Potato 项目文档"
    ET.SubElement(root, f"{{{dc}}}subject").text = "Unity 2D 波次生存游戏项目说明"
    ET.SubElement(root, f"{{{dc}}}creator").text = "Potato Project"
    ET.SubElement(root, f"{{{cp}}}keywords").text = "Unity, 2D, 波次生存, Potato"
    created = ET.SubElement(root, f"{{{dcterms}}}created")
    created.set(f"{{{xsi}}}type", "dcterms:W3CDTF")
    created.text = dt.datetime.now(dt.timezone.utc).replace(microsecond=0).isoformat().replace("+00:00", "Z")
    return ET.tostring(root, encoding="utf-8", xml_declaration=True)


def app_properties_xml() -> bytes:
    ep = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties"
    vt = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes"
    ET.register_namespace("", ep)
    ET.register_namespace("vt", vt)
    root = ET.Element(f"{{{ep}}}Properties")
    ET.SubElement(root, f"{{{ep}}}Application").text = "Microsoft Office Word"
    ET.SubElement(root, f"{{{ep}}}AppVersion").text = "16.0000"
    return ET.tostring(root, encoding="utf-8", xml_declaration=True)


def styles_xml() -> bytes:
    root = ET.Element(q("w", "styles"))
    doc_defaults = element(root, "w", "docDefaults")
    run_defaults = element(doc_defaults, "w", "rPrDefault")
    run_props = element(run_defaults, "w", "rPr")
    element(run_props, "w", "rFonts", w__ascii="Microsoft YaHei", w__hAnsi="Microsoft YaHei", w__eastAsia="Microsoft YaHei")
    element(run_props, "w", "sz", w__val="21")
    element(run_props, "w", "szCs", w__val="21")
    paragraph_defaults = element(doc_defaults, "w", "pPrDefault")
    default_props = element(paragraph_defaults, "w", "pPr")
    element(default_props, "w", "spacing", w__after="140", w__line="330", w__lineRule="auto")

    add_style(root, "Normal", "正文", size=21, color="3D2A26")
    add_style(root, "Title", "标题", size=52, color="67211E", bold=True, after=180)
    add_style(root, "Subtitle", "副标题", size=25, color="8D5F50", italic=True, after=240)
    add_style(root, "Heading1", "一级标题", size=34, color="67211E", bold=True, before=340, after=150, keep_next=True, outline=0)
    add_style(root, "Heading2", "二级标题", size=28, color="8D3B35", bold=True, before=260, after=120, keep_next=True, outline=1)
    add_style(root, "Heading3", "三级标题", size=24, color="A45148", bold=True, before=220, after=100, keep_next=True, outline=2)
    add_style(root, "Quote", "引用", size=20, color="693D35", italic=True, left=360, after=180)
    add_style(root, "Code", "代码", size=18, color="F5E9D3", left=180, right=180, before=80, after=180)
    add_style(root, "Caption", "题注", size=18, color="8A7067", italic=True, after=180)
    add_style(root, "ListParagraph", "列表", size=21, color="3D2A26", left=360, hanging=240, after=70)
    return xml_bytes(root)


def add_style(
    root: ET.Element,
    style_id: str,
    name: str,
    size: int,
    color: str,
    bold: bool = False,
    italic: bool = False,
    before: int = 0,
    after: int = 120,
    left: int = 0,
    right: int = 0,
    hanging: int = 0,
    keep_next: bool = False,
    outline: int | None = None,
) -> None:
    style = element(root, "w", "style", w__type="paragraph", w__styleId=style_id)
    element(style, "w", "name", w__val=name)
    if style_id == "Normal":
        element(style, "w", "qFormat")
    paragraph_props = element(style, "w", "pPr")
    element(paragraph_props, "w", "spacing", w__before=str(before), w__after=str(after), w__line="330", w__lineRule="auto")
    if left or right or hanging:
        element(paragraph_props, "w", "ind", w__left=str(left), w__right=str(right), w__hanging=str(hanging))
    if keep_next:
        element(paragraph_props, "w", "keepNext")
    if outline is not None:
        element(paragraph_props, "w", "outlineLvl", w__val=str(outline))
    run_props = element(style, "w", "rPr")
    element(run_props, "w", "rFonts", w__ascii="Microsoft YaHei", w__hAnsi="Microsoft YaHei", w__eastAsia="Microsoft YaHei")
    if bold:
        element(run_props, "w", "b")
    if italic:
        element(run_props, "w", "i")
    element(run_props, "w", "color", w__val=color)
    element(run_props, "w", "sz", w__val=str(size))
    element(run_props, "w", "szCs", w__val=str(size))


def settings_xml() -> bytes:
    root = ET.Element(q("w", "settings"))
    element(root, "w", "zoom", w__percent="100")
    element(root, "w", "defaultTabStop", w__val="420")
    element(root, "w", "updateFields", w__val="true")
    return xml_bytes(root)


def header_xml() -> bytes:
    root = ET.Element(q("w", "hdr"))
    paragraph = element(root, "w", "p")
    props = element(paragraph, "w", "pPr")
    element(props, "w", "jc", w__val="right")
    borders = element(props, "w", "pBdr")
    element(borders, "w", "bottom", w__val="single", w__sz="6", w__space="4", w__color="C9AA8D")
    DocxBuilder.add_run(paragraph, "POTATO  ·  项目文档", bold=True, color="8D3B35", size=17)
    return xml_bytes(root)


def footer_xml() -> bytes:
    root = ET.Element(q("w", "ftr"))
    paragraph = element(root, "w", "p")
    props = element(paragraph, "w", "pPr")
    element(props, "w", "jc", w__val="center")
    DocxBuilder.add_run(paragraph, "—  ", color="9A7D70", size=17)
    field = element(paragraph, "w", "fldSimple", w__instr="PAGE")
    run = element(field, "w", "r")
    run_props = element(run, "w", "rPr")
    element(run_props, "w", "color", w__val="9A7D70")
    element(run_props, "w", "sz", w__val="17")
    element(run, "w", "t").text = "1"
    DocxBuilder.add_run(paragraph, "  —", color="9A7D70", size=17)
    return xml_bytes(root)


def clean_inline(text: str) -> str:
    text = re.sub(r"!\[([^]]*)\]\([^)]+\)", r"\1", text)
    text = re.sub(r"\[([^]]+)\]\(<([^>]+)>\)", lambda m: f"{m.group(1)}（{urllib.parse.unquote(m.group(2))}）", text)
    text = re.sub(r"\[([^]]+)\]\(([^)]+)\)", lambda m: f"{m.group(1)}（{urllib.parse.unquote(m.group(2))}）", text)
    text = text.replace("**", "").replace("`", "")
    return text.strip()


def markdown_image_path(line: str) -> tuple[Path, str] | None:
    match = re.match(r"!\[([^]]*)\]\(<([^>]+)>\)", line.strip())
    if not match:
        match = re.match(r"!\[([^]]*)\]\(([^)]+)\)", line.strip())
    if not match:
        return None
    return ROOT / urllib.parse.unquote(match.group(2)), match.group(1)


def html_image_path(line: str) -> tuple[Path, str, float] | None:
    match = re.search(r'<img\s+src="([^"]+)"(?:\s+alt="([^"]*)")?(?:\s+width="([0-9]+)")?', line)
    if not match:
        return None
    pixel_width = float(match.group(3) or 640)
    return ROOT / urllib.parse.unquote(match.group(1)), match.group(2) or "UI 图片", min(6.0, pixel_width / 120.0)


def create_flowchart(path: Path) -> None:
    width, height = 1600, 1780
    image = Image.new("RGB", (width, height), "#F7ECDD")
    draw = ImageDraw.Draw(image)
    regular = ImageFont.truetype(r"C:\Windows\Fonts\msyh.ttc", 34)
    bold = ImageFont.truetype(r"C:\Windows\Fonts\msyhbd.ttc", 38)
    small = ImageFont.truetype(r"C:\Windows\Fonts\msyh.ttc", 27)
    accent = "#7B2F2B"
    ink = "#3D2A26"
    border = "#B57A66"

    draw.rounded_rectangle((35, 35, width - 35, height - 35), radius=36, outline="#D2B49B", width=5)
    draw.text((width // 2, 95), "完整游戏流程", font=bold, fill=accent, anchor="mm")

    boxes = [
        ("标题界面", "任意键 / 鼠标进入"),
        ("选择存档", "选择或创建 3 个独立存档槽"),
        ("主操作界面", "新游戏 / 继续 / 设置 / 退出"),
        ("角色选择", "选择角色、初始武器与属性配置"),
        ("波次战斗", "移动、自动攻击、收集材料与经验"),
        ("波后奖励", "等待 1 秒 → 升级奖励 → 箱子奖励"),
        ("商店整备", "购买、锁定、刷新、查看装备详情"),
        ("下一波", "等待 0.5 秒后进入后续战斗"),
    ]
    center_x = width // 2
    box_width, box_height, gap = 1050, 135, 55
    top = 180
    centers: list[tuple[int, int]] = []
    for index, (title, note) in enumerate(boxes):
        y1 = top + index * (box_height + gap)
        y2 = y1 + box_height
        x1, x2 = center_x - box_width // 2, center_x + box_width // 2
        fill = "#F1DCC3" if index % 2 == 0 else "#FFF9EF"
        draw.rounded_rectangle((x1, y1, x2, y2), radius=24, fill=fill, outline=border, width=4)
        draw.text((center_x, y1 + 43), title, font=bold, fill=accent, anchor="mm")
        draw.text((center_x, y1 + 95), note, font=small, fill=ink, anchor="mm")
        centers.append((center_x, (y1 + y2) // 2))
        if index < len(boxes) - 1:
            line_start = y2 + 8
            line_end = y2 + gap - 8
            draw.line((center_x, line_start, center_x, line_end), fill=accent, width=6)
            draw.polygon(
                [(center_x, line_end + 11), (center_x - 13, line_end - 10), (center_x + 13, line_end - 10)],
                fill=accent,
            )

    # Cycle arrow from next wave back to battle.
    battle_y = centers[4][1]
    next_y = centers[7][1]
    right_x = center_x + box_width // 2 + 100
    draw.line((center_x + box_width // 2, next_y, right_x, next_y), fill=accent, width=6)
    draw.line((right_x, next_y, right_x, battle_y), fill=accent, width=6)
    draw.line((right_x, battle_y, center_x + box_width // 2 + 8, battle_y), fill=accent, width=6)
    draw.polygon(
        [(center_x + box_width // 2 - 5, battle_y), (center_x + box_width // 2 + 18, battle_y - 13), (center_x + box_width // 2 + 18, battle_y + 13)],
        fill=accent,
    )
    draw.text((right_x + 18, (battle_y + next_y) // 2), "继续循环", font=small, fill=accent, anchor="lm")

    settle_y1, settle_y2 = 1620, 1725
    draw.rounded_rectangle((180, settle_y1, width - 180, settle_y2), radius=24, fill="#7B2F2B", outline="#61201E", width=4)
    draw.text((width // 2, settle_y1 + 36), "死亡 / 完成第 20 波", font=regular, fill="#FCEEDB", anchor="mm")
    draw.text((width // 2, settle_y1 + 78), "进入游戏结算，可重新开始或返回主菜单", font=small, fill="#FCEEDB", anchor="mm")
    image.save(path, "PNG", optimize=True)


def build_document() -> None:
    readme_lines = README.read_text(encoding="utf-8").splitlines()
    builder = DocxBuilder()

    # Cover
    builder.add_paragraph("POTATO", style="Title", align="center", before=450, after=100)
    builder.add_paragraph("2D 俯视角波次生存游戏 · 项目文档", style="Subtitle", align="center", after=220)
    builder.add_rule()
    cover_image = ROOT / "Assets/Arts/pohe2/back2.png"
    builder.add_image(cover_image, width_inches=6.25)
    builder.add_paragraph("完整主循环原型  ·  Unity 2022.3.62f2c1", align="center", bold=True, color="7B2F2B", size=22, after=100)
    builder.add_paragraph("标题与存档  /  角色选择  /  波次战斗  /  升级与箱子  /  商店  /  结算", align="center", color="6F544B", size=18, after=160)
    builder.add_table(
        [
            ["场景", "存档槽", "最终波次", "角色定义"],
            ["2", "3", "20", "4"],
        ],
        [2250, 2250, 2250, 2250],
    )
    builder.add_paragraph(f"文档生成日期：{dt.date.today().isoformat()}", align="center", color="8A7067", size=17)
    builder.add_page_break()
    builder.add_paragraph("项目概览", style="Heading1")

    index = 1  # Skip '# Potato'
    while index < len(readme_lines):
        raw = readme_lines[index]
        line = raw.strip()
        if not line:
            index += 1
            continue

        if line.startswith("```"):
            language = line[3:].strip().lower()
            block: list[str] = []
            index += 1
            while index < len(readme_lines) and not readme_lines[index].strip().startswith("```"):
                block.append(readme_lines[index])
                index += 1
            if language == "mermaid":
                with tempfile.TemporaryDirectory() as temp_dir:
                    flow_path = Path(temp_dir) / "game-flow.png"
                    create_flowchart(flow_path)
                    builder.add_image(flow_path, width_inches=6.1, caption="图 1　游戏主循环")
            else:
                builder.add_code(block)
            index += 1
            continue

        image_info = markdown_image_path(line)
        if image_info:
            path, alt = image_info
            builder.add_image(path, width_inches=6.15, caption=alt)
            index += 1
            continue

        html_image = html_image_path(line)
        if html_image:
            path, alt, width_inches = html_image
            builder.add_image(path, width_inches=width_inches, caption=alt)
            index += 1
            continue
        if line.startswith("<p") or line.startswith("</p"):
            index += 1
            continue

        if line.startswith("|") and index + 1 < len(readme_lines):
            table_rows: list[list[str]] = []
            while index < len(readme_lines) and readme_lines[index].strip().startswith("|"):
                values = [cell.strip() for cell in readme_lines[index].strip().strip("|").split("|")]
                if not all(re.fullmatch(r":?-{3,}:?", value) for value in values):
                    table_rows.append(values)
                index += 1
            builder.add_table(table_rows)
            continue

        heading = re.match(r"^(#{1,6})\s+(.+)$", line)
        if heading:
            markdown_level = len(heading.group(1))
            word_level = min(3, max(1, markdown_level - 1))
            builder.add_paragraph(clean_inline(heading.group(2)), style=f"Heading{word_level}")
            index += 1
            continue

        if line.startswith(">"):
            builder.add_quote(clean_inline(line.lstrip("> ")))
            index += 1
            continue

        bullet = re.match(r"^[-*]\s+(.+)$", line)
        if bullet:
            builder.add_paragraph(f"•  {clean_inline(bullet.group(1))}", style="ListParagraph")
            index += 1
            continue

        numbered = re.match(r"^(\d+)\.\s+(.+)$", line)
        if numbered:
            builder.add_paragraph(f"{numbered.group(1)}.  {clean_inline(numbered.group(2))}", style="ListParagraph")
            index += 1
            continue

        paragraph_lines = [line]
        index += 1
        while index < len(readme_lines):
            candidate = readme_lines[index].strip()
            if not candidate:
                break
            if (
                candidate.startswith(("#", ">", "```", "|", "- ", "* ", "![", "<p", "</p", "<img"))
                or re.match(r"^\d+\.\s+", candidate)
            ):
                break
            paragraph_lines.append(candidate)
            index += 1
        builder.add_paragraph(clean_inline(" ".join(paragraph_lines)))

    builder.write(OUTPUT)


if __name__ == "__main__":
    build_document()
    print(f"Generated: {OUTPUT}")
    print(f"Size: {OUTPUT.stat().st_size} bytes")
